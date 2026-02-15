using StackExchange.Redis;
using XEvent.Reservation.Domain;
using XEvent.Reservation.UseCaseHandlers;

namespace XEvent.Reservation.ACL;

public class RedisTicketCapacityStore : ITicketCapacityStore
{
    private readonly IConnectionMultiplexer _redis;

    public RedisTicketCapacityStore(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    private static string CapacityKey(EventId e, TicketTypeId t)
        => $"capacity:{e.Value}:{t.Value}";

    private static string ReservationKey(EventId e, TicketTypeId t)
        => $"reservations:{e.Value}:{t.Value}";

    public async Task<bool> TryReserveAsync(
        ReservationId reservationId,
        EventId eventId,
        TicketTypeId ticketTypeId,
        int quantity,
        TimeSpan ttl,
        CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        var capacityKey = CapacityKey(eventId, ticketTypeId);
        var reservationKey = ReservationKey(eventId, ticketTypeId);

        var expireAt = DateTimeOffset.UtcNow.Add(ttl).ToUnixTimeSeconds();

        var script = @"
local expired = redis.call('ZRANGEBYSCORE', KEYS[2], '-inf', ARGV[2])
for i=1,#expired do
  local parts = {}
  for s in string.gmatch(expired[i], '[^:]+') do
    table.insert(parts, s)
  end
  redis.call('INCRBY', KEYS[1], tonumber(parts[2]))
  redis.call('ZREM', KEYS[2], expired[i])
end

local available = tonumber(redis.call('GET', KEYS[1]) or '-1')
if available < tonumber(ARGV[1]) then
  return 0
end

redis.call('DECRBY', KEYS[1], ARGV[1])
local payload = ARGV[3] .. ':' .. ARGV[1]
redis.call('ZADD', KEYS[2], ARGV[2], payload)
return 1
";

        var result = (int)await db.ScriptEvaluateAsync(
            script,
            new RedisKey[] { capacityKey, reservationKey },
            new RedisValue[]
            {
                quantity,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                reservationId.Value
            });

        return result == 1;
    }


    public async Task ReleaseAsync(
        ReservationId reservationId,
        EventId eventId,
        TicketTypeId ticketTypeId,
        CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        var capacityKey = CapacityKey(eventId, ticketTypeId);
        var reservationKey = ReservationKey(eventId, ticketTypeId);

        var entries = await db.SortedSetRangeByScoreAsync(reservationKey);
        foreach (var entry in entries)
        {
            var parts = entry.ToString().Split(':');
            if (parts[0] == reservationId.Value.ToString())
            {
                var quantity = int.Parse(parts[1]);
                await db.SortedSetRemoveAsync(reservationKey, entry);
                await db.StringIncrementAsync(capacityKey, quantity);
                return;
            }
        }
    }
    public async Task ConfirmAsync(
        ReservationId reservationId,
        EventId eventId,
        TicketTypeId ticketTypeId,
        CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        var reservationKey = ReservationKey(eventId, ticketTypeId);

        var entries = await db.SortedSetRangeByScoreAsync(reservationKey);
        foreach (var entry in entries)
        {
            var parts = entry.ToString().Split(':');
            if (parts[0] == reservationId.Value.ToString())
            {
                await db.SortedSetRemoveAsync(reservationKey, entry);
                return;
            }
        }
    }

    public async Task UpdateCapacityAsync(
        EventId eventId,
        TicketTypeId ticketTypeId,
        int newCapacity,
        CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        await db.StringSetAsync(CapacityKey(eventId, ticketTypeId), newCapacity);
    }
}