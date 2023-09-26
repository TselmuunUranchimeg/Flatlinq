using System.ComponentModel.DataAnnotations;

namespace Flatlinq.Models.DTO;

public enum ExternalAuthenticationEnum
{
    Facebook,
    Google
}

public class LoginDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class RegisterDTO : LoginDTO
{
    [Required]
    [DataType(DataType.Text)]
    public string Username { get; set; } = "";
}

public class TokenReturnDTO
{
    public string RefreshToken { get; set; } = "";
    public string AccessToken { get; set; } = "";
}

public class RoleDTO
{
    [Required]
    public bool IsTenant { get; set; }
}