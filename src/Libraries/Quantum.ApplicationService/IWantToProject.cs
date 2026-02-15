using Quantum.Domain;

namespace Quantum.ApplicationService;

public interface IWantToProject<in T>
where T:IsADomainEvent
{
    Task On(T @event, CancellationToken ct);
}