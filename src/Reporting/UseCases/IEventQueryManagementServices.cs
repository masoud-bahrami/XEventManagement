using Quantum.ApplicationService;
using XEvent.EventManagement.Domain;
using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.Reporting;

public interface IEventQueryManagementServices
{
    Task<IReadOnlyCollection<EventViewModel>> All();
    Task<PagedResult<EventViewModel>> AllPublicEvents(string baseApiAddress, int page, int pageSize, string? nameFilter, DateTime? @from, DateTime? to);
}

public sealed class EventQueryManagementService : IEventQueryManagementServices
{
    private readonly IEventRepository _repository;
    private readonly ICurrentUser _currentUser;

    public EventQueryManagementService(
        IEventRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }
    
    public async Task<IReadOnlyCollection<EventViewModel>> All()
    {
        return await _repository.LoadAll(_currentUser.UserId);
    }


    public async Task<PagedResult<EventViewModel>> AllPublicEvents(string baseApiAddress,
        int page, int pageSize, string? nameFilter = null, DateTime? from = null, DateTime? to = null)
    {
        var events = await _repository.LoadAll();

        var query = events.Where(e => e.Status == Status.Published);

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(e => e.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));

        if (from.HasValue)
            query = query.Where(e => e.StartDate >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.EndDate <= to.Value);

        var totalCount = query.Count();
        var pageInfo = new PageInfo(page, pageSize, totalCount);

        var eventViewModels = query.Select(e => new EventViewModel(
            e.Id,
            e.Owner,
            e.Name,
            e.Tickets,
            e.StartDate,
            e.EndDate,
            e.Status
        ));

        var (pagedItems, links) = PaginationHelper.PaginateAndLink(
            eventViewModels,
            pageInfo,
            "/api/events/public",
            new Dictionary<string, string>
            {
                ["nameFilter"] = nameFilter ?? "",
                ["from"] = from?.ToString("yyyy-MM-dd") ?? "",
                ["to"] = to?.ToString("yyyy-MM-dd") ?? ""
            }
        );

        return new PagedResult<EventViewModel>(pagedItems, page, pageSize, totalCount, links);
    }
    
}