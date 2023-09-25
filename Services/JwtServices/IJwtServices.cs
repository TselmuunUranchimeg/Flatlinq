namespace Flatlinq.Services;

public interface IJwtServices
{
    string GenerateToken(User user, bool isAccessToken, string role);
    string GetIdFromToken(string tokenString);
}