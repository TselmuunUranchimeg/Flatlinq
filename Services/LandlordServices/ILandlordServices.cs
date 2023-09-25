namespace Flatlinq.Services;

public interface ILandlordServices
{
    ServiceDTO GetRecommendation(string accessToken, int position);
}