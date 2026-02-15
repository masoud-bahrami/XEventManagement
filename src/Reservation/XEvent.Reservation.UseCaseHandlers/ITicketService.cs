namespace XEvent.Reservation.UseCaseHandlers;

public interface ITicketService
{
    string GenerateQRCode(long reservationId, ReservedAttendee attendee);
    byte[] GeneratePdfTicket(long reservationId, ReservedAttendee attendee);
    byte[] GeneratePdfTickets(Domain.Reservation reservation);
}