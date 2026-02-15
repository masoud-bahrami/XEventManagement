using System.Text.Json;

namespace XEvent.Reservation.UseCaseHandlers;

public class TicketService : ITicketService
{
    public string GenerateQRCode(long reservationId, ReservedAttendee attendee)
    {
        var qrBytes = System.Text.Encoding.UTF8.GetBytes(QRCode(reservationId, attendee.Id));
        return Convert.ToBase64String(qrBytes);
    }

    
    public byte[] GeneratePdfTicket(long reservationId, ReservedAttendee attendee)
    {
        var text = $"Ticket for {attendee.FirstName} {attendee.LastName}\nQR: {QRCode(reservationId, attendee.Id)}";
        return System.Text.Encoding.UTF8.GetBytes(text);
    }

    public byte[] GeneratePdfTickets(Domain.Reservation reservation)
    {
        using var ms = new MemoryStream();
        foreach (var attendee in reservation.Attendees)
        {
            var bytes = GeneratePdfTicket(reservation.Id.Value, attendee);
            ms.Write(bytes, 0, bytes.Length);
            ms.WriteByte((byte)'\n');
        }
        return ms.ToArray();
    }

    private string QRCode(long reservationId, long attendeeId)
    {
        var payload = new
        {
            ReservationId = reservationId,
            AttendeeId = attendeeId
        };

        return JsonSerializer.Serialize(payload);
    }
}