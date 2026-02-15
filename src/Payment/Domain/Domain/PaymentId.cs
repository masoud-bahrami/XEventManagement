using Quantum.Domain;

namespace XEvent.Payment.Domain;

public record PaymentId(long Value) : IsAnAggregateRootId(Value);