using Flatlinq.Data;

namespace Flatlinq.Services;

public class ServiceDTO
{
    public IEnumerable<LandlordGetRecommendationDTO> Data { get; set; } = null!;
    public bool IsFinished { get; set; }
}

public class LandlordServices : ILandlordServices
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IJwtServices _jwtServices;

    public LandlordServices(CoreDbContext coreDbContext, IJwtServices jwtServices)
    {
        _coreDbContext = coreDbContext;
        _jwtServices = jwtServices;
    }
    public ServiceDTO GetRecommendation(string accessToken, int position)
    {
        string userId = _jwtServices.GetIdFromToken(accessToken);
        Landlord landlord = _coreDbContext.Landlords!.Where(v => v.UserId == userId).FirstOrDefault()!;
        IQueryable<Tenant> tenants = _coreDbContext.Tenants!.AsQueryable();
        var swipesWithHouses = from house in landlord.Houses
                               from swipe in _coreDbContext.Swipes!.Where(s => s.HouseId == house.Id)
                               select new { swipe, house };
        var target = from tenant in tenants
                     from swipe in swipesWithHouses.Where(s => s.swipe.SwiperId == tenant.UserId)
                     select new LandlordGetRecommendationDTO
                     {
                         Name = tenant.User.UserName!,
                         UserId = tenant.User.Id
                     };
        bool isFinished = false;
        if (position > target.Count())
        {
            target = from alreadySwiped in target
                     from tenant in _coreDbContext.Tenants.Where(x => x.UserId != alreadySwiped.UserId)
                     select new LandlordGetRecommendationDTO
                     {
                         Name = tenant.User.UserName!,
                         UserId = tenant.User.Id
                     };
            target = target.Take(10);
            isFinished = true;
        }
        else
        {
            target = target
                .Skip(position)
                .Take(10);
        }
        return new ServiceDTO
        {
            Data = target,
            IsFinished = isFinished
        };
    }
}