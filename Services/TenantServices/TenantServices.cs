using Flatlinq.Data;
using Flatlinq.Models.DTO;

namespace Flatlinq.Services;

public class TenantServices: ITenantServices
{
    private readonly CoreDbContext _coreDbContext;
    public TenantServices(CoreDbContext coreDbContext)
    {
        _coreDbContext = coreDbContext;
    }

    public IQueryable<House> GetRecommendation(GetRecommendationDTO data, int position)
    {
        IQueryable<House> query = _coreDbContext.Houses!.AsQueryable();
        query = data.MinPrice != null ? query.Where(v => v.Price >= data.MinPrice) : query;
        query = data.MaxPrice != null ? query.Where(v => v.Price <= data.MaxPrice) : query;
        query = data.AllowChildren != null ? query.Where(v => v.AllowChildren) : query;
        query = data.AllowPets != null ? query.Where(v => v.AllowPets) : query;
        query = data.HasElectricity != null ? query.Where(v => v.HasElectricity) : query;
        query = data.HasInternet != null ? query.Where(v => v.HasInternet) : query;
        query = query
            .Skip(position + 10 > query.Count() ? position : query.Count() - position)
            .Take(position + 10 > query.Count() ? 10 : query.Count() - position);
        return query;
    }
}