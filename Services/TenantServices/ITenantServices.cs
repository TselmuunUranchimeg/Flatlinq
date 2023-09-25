using Flatlinq.Models.DTO;

namespace Flatlinq.Services;

public interface ITenantServices
{
    IQueryable<House> GetRecommendation(GetRecommendationDTO data, int position);
}