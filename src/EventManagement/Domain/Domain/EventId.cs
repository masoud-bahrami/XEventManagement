using Quantum.Domain;

namespace XEvent.EventManagement.Domain;

public sealed record EventId(long value) : IsAnAggregateRootId(value);