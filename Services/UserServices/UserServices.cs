using Microsoft.AspNetCore.Identity;
using Stripe;

namespace Flatlinq.Services;

public class UserServices: IUserServices
{
    private readonly IJwtServices _jwtServices;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    public UserServices(UserManager<User> userManager, 
        IJwtServices jwtServices, IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtServices = jwtServices;
        _configuration = configuration;
    }
    public async Task SubscribeToGold(string accessToken)
    {
        string userId = _jwtServices.GetIdFromToken(accessToken);
        User user = await _userManager.FindByIdAsync(userId) ?? throw new MyException("User doesn't exist!");
        Dictionary<string, string> metadata = new()
        {
            { "userId", userId }
        };
        CustomerCreateOptions customerCreateOptions = new()
        {
            Email = user.Email,
            Name = user.UserName,
            Metadata = metadata
        };
        CustomerService customerService = new();
        Customer customer = await customerService.CreateAsync(customerCreateOptions);
        SubscriptionService subscriptionService = new();
        SubscriptionCreateOptions subscribeCreateOptions = new()
        {
            Customer = customer.Id,
            Items = new List<SubscriptionItemOptions>
            {
                new()
                {
                    Plan = _configuration["PlanId"]
                }
            }
        };
        await subscriptionService.CreateAsync(subscribeCreateOptions);
    }

    public void VerifyBankId()
    {
        
    }
}