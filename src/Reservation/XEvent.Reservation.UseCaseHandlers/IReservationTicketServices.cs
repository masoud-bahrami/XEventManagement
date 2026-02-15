using XEvent.Reservation.Domain;

namespace XEvent.Reservation.UseCaseHandlers;

public interface IReservationTicketServices
{
    Task<byte[]> GeneratePdfTickets(long reservationId);
    Task<byte[]> GeneratePdfTicket(long reservationId, long attendeeId);
}

public class ReservationTicketServices : IReservationTicketServices
{
    private readonly IReservationRepository _repository;
    private readonly ITicketService _ticketService;
    public ReservationTicketServices(IReservationRepository repository, ITicketService ticketService)
    {
        _repository = repository;
        _ticketService = ticketService;
    }

    public async Task<byte[]> GeneratePdfTickets(long reservationId)
    {
        var reservation = await _repository.GetAsync(new ReservationId(reservationId), CancellationToken.None);
        if (reservation == null) throw new Exception();

        var pdf = _ticketService.GeneratePdfTickets(reservation);

        return pdf;
    }

    public async Task<byte[]> GeneratePdfTicket(long reservationId, long attendeeId)
    {
        var reservation = await _repository.GetAsync(new ReservationId(reservationId), CancellationToken.None);
        if (reservation == null) throw new Exception();

        var attendee = reservation.Attendees.FirstOrDefault(a => a.Id == attendeeId);
        if (attendee == null) throw new Exception();

        var pdf = _ticketService.GeneratePdfTicket(reservationId, attendee);

        return pdf;
    }
}