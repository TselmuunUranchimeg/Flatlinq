using Flatlinq.Models.DTO;

namespace Flatlinq.Services;

public interface IAuthServices
{
    Task<TokenReturnDTO?> RegisterUser(RegisterDTO data);
    Task SetRole(string role, string? tokenHeader);
    Task<TokenReturnDTO?> LoginUser(LoginDTO data);
    Task<TokenReturnDTO> VerifyTokens(string refreshToken);
    Task<TokenReturnDTO> ExternalAuthentication(string accessToken, ExternalAuthenticationEnum authenticationEnum);
}