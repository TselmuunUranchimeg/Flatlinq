using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace Flatlinq.Services;

public class UserServices: IUserServices
{
    private readonly IJwtServices _jwtServices;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    public UserServices(UserManager<User> userManager, IJwtServices jwtServices, 
        IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _jwtServices = jwtServices;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
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
        user.IsGoldMember = true;
        await _userManager.UpdateAsync(user);
    }

    public async Task<StartBankIdVerificationDTO> StartBankIdVerification(string endUserIp, string accessToken)
    {
        using HttpClient client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        var requestBody = new { endUserIp };
        HttpResponseMessage res = await client.PostAsJsonAsync(
            "https://appapi2.bankid.com/auth", JsonSerializer.Serialize(requestBody));
        res.EnsureSuccessStatusCode();
        Dictionary<string, string> data = JsonSerializer.Deserialize<Dictionary<string, string>>(await res.Content.ReadAsStringAsync())!;
        PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(2));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            HttpResponseMessage collectRes = await client.PostAsJsonAsync
            (
                "https://appapi2.bankid.com/collect",
                JsonSerializer.Serialize(new
                {
                    orderRef = data["orderRef"]!
                })
            );
            Dictionary<string, string> collectData = JsonSerializer.Deserialize<Dictionary<string, string>>(await collectRes.Content.ReadAsStringAsync())!;
            if (collectData["status"] == "complete")
            {
                string userId = _jwtServices.GetIdFromToken(accessToken);
                User? user = await _userManager.FindByIdAsync(userId) ?? throw new MyException("User doesn't exist");
                user.BankIdVerified = true;
                await _userManager.UpdateAsync(user);
            }
        }
        return new StartBankIdVerificationDTO
        {
            AutoStartToken = data["autoStartToken"]!,
            OrderRef = data["orderRef"]!
        };
    }
}