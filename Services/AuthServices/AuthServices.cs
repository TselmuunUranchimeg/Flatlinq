using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Flatlinq.Models.DTO;
using Flatlinq.Data;

namespace Flatlinq.Services;

public class AuthServices : IAuthServices
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtServices _jwtServices;
    private readonly CoreDbContext _coreDbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthServices(UserManager<User> userManager, IJwtServices jwtServices, RoleManager<IdentityRole> roleManager,
        CoreDbContext coreDbContext, IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _jwtServices = jwtServices;
        _roleManager = roleManager;
        _coreDbContext = coreDbContext;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Returns access and refresh tokens in an array.
    /// </summary>
    /// <param name="user">
    /// User object
    /// </param>
    /// <returns>
    /// String array. First element is refresh token, while the second one is the access token.
    /// </returns>
    private async Task<string[]> GenerateTokens(User user)
    {
        return await Task.WhenAll(new Task<string>[]
        {
            //Refresh token
            Task.Run(() =>
            {
                return _jwtServices.GenerateToken(user, false, "");
            }),

            //Access token
            Task.Run(() =>
            {
                return _jwtServices.GenerateToken(user, true, "");
            })
        });
    }

    public async Task<TokenReturnDTO?> RegisterUser(RegisterDTO data)
    {
        if (await _userManager.FindByEmailAsync(data.Email) is null)
        {
            User newUser = new()
            {
                Email = data.Email,
                UserName = data.Username
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, data.Password);
            if (result.Errors.Count() == 0)
            {
                string[] tokens = await GenerateTokens(newUser);
                return new TokenReturnDTO
                {
                    RefreshToken = tokens[0],
                    AccessToken = tokens[1]
                };
            }
            throw new Exception(result.Errors.First().Description);
        }
        return null;
    }

    public async Task SetRole(string role, string? tokenHeader)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            Console.WriteLine("Result is false");
            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = role
            });
        }
        if (tokenHeader is not null)
        {
            string userId = _jwtServices.GetIdFromToken(tokenHeader);
            Console.WriteLine(userId);
            if (userId != string.Empty)
            {
                User? user = await _userManager.FindByIdAsync(userId);
                if (user is not null)
                {
                    if (role == "Tenant")
                    {
                        _coreDbContext.Tenants?.Add(new Tenant()
                        {
                            User = user
                        }
                        );
                    }
                    else if (role == "Landlord")
                    {
                        _coreDbContext.Landlords?.Add(new Landlord()
                        {
                            User = user
                        });
                    }
                    await Task.WhenAll(new Task[]
                    {
                        _coreDbContext.SaveChangesAsync(),
                        _userManager.AddToRoleAsync(user, role)
                    });
                    return;
                }
                throw new MyException("User doesn't exist");
            }
        }
        throw new MyException("Authorization header missing!");
    }

    public async Task<TokenReturnDTO?> LoginUser(LoginDTO data)
    {
        User? user = await _userManager.FindByEmailAsync(data.Email);
        if (user != null)
        {
            if (await _userManager.CheckPasswordAsync(user, data.Password))
            {
                string[] tokens = await GenerateTokens(user);
                return new TokenReturnDTO
                {
                    AccessToken = tokens[1],
                    RefreshToken = tokens[0]
                };
            }
        }
        return null;
    }

    public async Task<TokenReturnDTO> VerifyTokens(string refreshToken)
    {
        string userId = _jwtServices.GetIdFromToken(refreshToken);
        User? user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            string[] tokens = await GenerateTokens(user);
            return new TokenReturnDTO
            {
                AccessToken = tokens[1],
                RefreshToken = tokens[0]
            };
        }
        throw new MyException("User doesn't exist");
    }

    public async Task<TokenReturnDTO> ExternalAuthentication(string accessToken, ExternalAuthenticationEnum authenticationEnum)
    {
        using HttpClient client = _httpClientFactory.CreateClient();
        if (authenticationEnum == ExternalAuthenticationEnum.Google)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }
        string link = authenticationEnum == ExternalAuthenticationEnum.Facebook
        ? $"https://graph.facebook.com/v18.0/me?access_token={accessToken}"
        : "https://www.googleapis.com/oauth2/v3/userinfo";
        HttpResponseMessage response = await client.GetAsync(link);
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(
            await response.Content.ReadAsStringAsync())!;
        if ((await _userManager.FindByEmailAsync(data["email"])) != null)
        {
            throw new MyException("User already exists!");
        }
        User user = new()
        {
            UserName = data["name"],
            Email = data["email"]
        };
        IdentityResult result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            throw new MyException("Something went wrong, please try again later!");
        }
        string[] tokens = await GenerateTokens(user);
        return new TokenReturnDTO
        {
            AccessToken = tokens[1],
            RefreshToken = tokens[0]
        };
    }
}