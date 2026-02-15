using FluentAssertions;
using XEvent.Tickets.Domain.Contracts.Events;
using Xunit;

namespace XEvent.Tickets.Domain.Tests;

public class TicketTests
{
    [Fact]
    public void Create_ValidTicket_ShouldSucceed()
    {
        var ticket = new Ticket(1, "VIP", 100, 50);

        ticket.Name.Should().Be("VIP");
        ticket.Price.Should().Be(100);
        ticket.Capacity.Should().Be(50);
    }

    [Theory]
    [InlineData("", 10, 1)]
    [InlineData("VIP", -1, 1)]
    [InlineData("VIP", 10, 0)]
    public void Create_InvalidTicket_ShouldFail(string name, decimal price, short capacity)
    {
        Action act = () => new Ticket(1, name, price, capacity);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Tickets_WithDuplicateNames_ShouldFail()
    {
        var tickets = new List<TicketDto>
        {
            new("VIP", 100, 10),
            new("VIP", 200, 10)
        };

        Action act = () => CreateTickets(tickets);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Ticket with name 'VIP' already exists");
    }

    [Fact]
    public void Tickets_ApplyAddEvent_ShouldAddTicket()
    {
        var tickets = CreateTickets(new List<TicketDto> {new("VIP", 100, 10)});

        var addEvent = new NewEventTicketIsAddedEvent(2,1, new TicketDto("Standard", 50, 20));
        tickets.Apply(addEvent);

        tickets.Count.Should().Be(2);
        tickets.Any(t => t.Name == "Standard").Should().BeTrue();
    }

    [Fact]
    public void Tickets_ApplyUpdateEvent_ShouldUpdateTicket()
    {
        var tickets = CreateTickets(new List<TicketDto> {new("VIP", 100, 10)});

        var updateEvent = new EventTicketIsUpdatedEvent(1, tickets.Single().Id, new TicketDto("VIP Updated", 150, 15));
        tickets.Apply(updateEvent);

        var updated = tickets.First();
        updated.Name.Should().Be("VIP Updated");
        updated.Price.Should().Be(150);
        updated.Capacity.Should().Be(15);
    }

    [Fact]
    public void Tickets_ApplyRemoveEvent_ShouldRemoveTicket()
    {
        var tickets = CreateTickets(new List<TicketDto> {new("VIP", 100, 10)});

        var removeEvent = new ATicketIsRemovedFromEvent(1, tickets.Single().Id);
        tickets.Apply(removeEvent);

        tickets.Count.Should().Be(0);
    }

    [Fact]
    public void Tickets_ApplyUpdateEvent_OnNonExistingTicket_ShouldThrow()
    {
        var tickets = CreateTickets(new List<TicketDto> {new("VIP", 100, 10)});
        var updateEvent = new EventTicketIsUpdatedEvent(1, 999, new TicketDto("NonExistent", 50, 5));

        Action act = () => tickets.Apply(updateEvent);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Ticket with ID 999 not found*");
    }

    [Fact]
    public void Tickets_ApplyAddEvent_WithDuplicateName_ShouldThrow()
    {
        var tickets = CreateTickets(new List<TicketDto> {new("VIP", 100, 10)});
        var addEvent = new NewEventTicketIsAddedEvent(2,1, new TicketDto("VIP", 50, 5));

        Action act = () => tickets.Apply(addEvent);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Ticket with name 'VIP' already exists*");
    }

    private Tickets CreateTickets(List<TicketDto> dtos) => new(new TicketId(1), dtos);
}
