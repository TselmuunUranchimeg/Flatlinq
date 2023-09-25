namespace Flatlinq.Services;

public interface IUserServices
{
    Task SubscribeToGold(string accessToken);
}