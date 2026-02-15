using FluentAssertions;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.Domain.Contract.Events;
using Xunit;

namespace XEvent.EventManagement.Domain.Tests;

public class EventTests
{
    [Fact]
    public void CreateEvent_WithValidCommand_ShouldCreateDraftEventAndRaiseEventCreated()
    {
        var command = EventTestFactory.CreateCreateCommand();
        var sut = new Event(1, 1, command);
        var events = sut.GetUncommittedEvents();

        sut.Owner.Should().Be(1);
        sut.Name.Should().Be("My Event");
        sut.Status.Should().Be(Status.Draft);

        sut.Tickets.Should().HaveCount(1)
            .And.ContainSingle(t => t.Name == "Standard" &&
                                     t.Price == 25_000 &&
                                     t.Capacity == 100);

        events.Should()
            .Contain(e => e is EventCreated)
            .Which.Should()
            .BeOfType<EventCreated>()
            .Which.EventId.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateEvent_WithInvalidTicketCapacity_ShouldThrow(short capacity)
    {
        var command = EventTestFactory.CreateCreateCommand(capacity: capacity);
        Action act = () => new Event(1, 1, command);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateEvent_WhenEndDateIsBeforeStartDate_ShouldThrow()
    {
        var command = EventTestFactory.CreateCreateCommand(
            start: DateTime.UtcNow.AddDays(2),
            end: DateTime.UtcNow.AddDays(1));
        Action act = () => new Event(1, 1, command);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateEvent_ShouldChangeStateAndRaiseEventUpdated()
    {
        var sut = EventTestFactory.CreateDraftEvent();
        sut.ClearUncommittedEvents();

        var cmd = new UpdateEventCommand(1, "Updated Event",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(4));

        sut.Handle(cmd);
        var events = sut.GetUncommittedEvents();

        sut.Name.Should().Be("Updated Event");
        sut.Duration.Start.Date.Should().Be(cmd.StartDate.Date);
        sut.Duration.End.Date.Should().Be(cmd.EndDate.Date);

        events.Should().ContainSingle()
            .Which.Should().BeOfType<EventUpdated>()
            .Which.Name.Should().Be("Updated Event");
    }

    [Fact]
    public void PublishEvent_ShouldSetStatusAndRaiseEvent()
    {
        var sut = EventTestFactory.CreateDraftEvent();
        sut.ClearUncommittedEvents();

        sut.Handle(new PublishEventCommand(1));
        sut.Status.Should().Be(Status.Published);

        sut.GetUncommittedEvents().Should()
            .ContainSingle()
            .Which.Should().BeOfType<EventPublished>();
    }

    [Fact]
    public void CancelEvent_ShouldSetStatusAndRaiseEvent()
    {
        var sut = EventTestFactory.CreateDraftEvent();
        sut.ClearUncommittedEvents();

        sut.Handle(new CancelEventCommand(1));
        sut.Status.Should().Be(Status.Cancelled);

        sut.GetUncommittedEvents().Should()
            .ContainSingle()
            .Which.Should().BeOfType<EventCancelled>();
    }

    [Fact]
    public void DeleteEvent_ShouldRaiseEventDeleted()
    {
        var sut = EventTestFactory.CreateDraftEvent();
        sut.ClearUncommittedEvents();

        sut.Handle(new DeleteEventCommand(1));

        sut.GetUncommittedEvents()
            .Should()
            .ContainSingle()
            .Which.Should().BeOfType<EventDeleted>()
            .Which.EventId.Should().Be(1);
    }

    [Fact]
    public void UpdateSingleTicket_ShouldRaiseEventAndUpdateTicket()
    {
        var sut = EventTestFactory.CreateDraftEvent();
        sut.ClearUncommittedEvents();

        var ticket = sut.Tickets.First();
        const short newCapacity = 150;
        const int newPrice = 50_000;
        const string newName = "VIP";

        var updateCommand = new UpdateTicketsCommand(
            new TicketDto(newName, newPrice, newCapacity)
        );

        sut.Handle(ticket.Id, updateCommand);
        var events = sut.GetUncommittedEvents();

        // Check domain event
        events.Should().ContainSingle()
            .Which.Should().BeOfType<EventTicketIsUpdatedEvent>()
            .Which.TicketId.Should().Be(ticket.Id);

        // Apply event to Tickets to verify state
        sut.Tickets.Apply((EventTicketIsUpdatedEvent)events.First());
        var updatedTicket = sut.Tickets.First();
        updatedTicket.Name.Should().Be(newName);
        updatedTicket.Price.Should().Be(newPrice);
        updatedTicket.Capacity.Should().Be(newCapacity);
    }

    [Fact]
    public void AddNewTicket_ShouldRaiseEventAndAddTicket()
    {
        var sut = EventTestFactory.CreateDraftEvent();
        sut.ClearUncommittedEvents();

        var addCommand = new AddNewTicketCommand(new TicketDto("VIP", 80_000, 50));
        sut.Handle(addCommand);

        var events = sut.GetUncommittedEvents();
        events.Should().ContainSingle().Which.Should().BeOfType<NewEventTicketIsAddedEvent>();
        
        sut.Tickets.Apply(new NewEventTicketIsAddedEvent(2, new TicketDto("Name", 20, 20)));
        sut.Tickets.Should().ContainSingle(t => t.Name == "VIP" && t.Capacity == 50);
    }

    [Fact]
    public void DeleteTicket_ShouldRaiseEventAndRemoveTicket()
    {
        var sut = EventTestFactory.CreateDraftEvent();
        sut.ClearUncommittedEvents();

        var ticket = sut.Tickets.First();
        sut.Handle(new DeleteTicketCommand(ticket.Id));

        var events = sut.GetUncommittedEvents();
        events.Should().ContainSingle().Which.Should().BeOfType<ATicketIsRemovedFromEvent>();

        sut.Tickets.Apply((ATicketIsRemovedFromEvent)events.First());
        sut.Tickets.Should().BeEmpty();
    }

    [Fact]
    public void Handle_ShouldThrow_WhenEventIsPublished_OnTicketUpdate()
    {
        var sut = EventTestFactory.CreateCanceledEvent();
        var ticket = sut.Tickets.First();
        var command = EventTestFactory.CreateUpdateTicketsCommand();

        Action act = () => sut.Handle(ticket.Id, command);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot update a published event");
    }

    private Event CreateSampleEvent()
    {
        var cmdCreate = new CreateNewEventCommand(
            "My Event",
            new List<TicketDto> { new TicketDto("name", 25000, 100) },
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2)
        );
        return new Event(1, 1, cmdCreate);
    }
}
