using Quantum.Domain;
using XEvent.Tickets.Domain.Contracts.Events;

namespace XEvent.Tickets.Domain;

public sealed class Ticket : IsAnEntity<long>
{
    public decimal Price { get; private set; }
    public string Name { get; private set; }
    public short Capacity { get; private set; }

    public Ticket(long id, string name, decimal price, short capacity) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty");
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");
        if (price < 0)
            throw new ArgumentException("Price cannot be negative");

        Name = name.Trim();
        Price = price;
        Capacity = capacity;
    }

    public void Update(TicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name cannot be null or empty");
        if (dto.Capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");
        if (dto.Price < 0)
            throw new ArgumentException("Price cannot be negative");

        Name = dto.Name.Trim();
        Price = dto.Price;
        Capacity = dto.Capacity;
    }
}
