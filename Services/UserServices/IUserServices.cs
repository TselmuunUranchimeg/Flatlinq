namespace Flatlinq.Services;

public interface IUserServices
{
    Task SubscribeToGold(string accessToken);
    Task<StartBankIdVerificationDTO> StartBankIdVerification(string endUserIp, string accessToken);
}