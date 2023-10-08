using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flatlinq.Models.DTO;
using Flatlinq.Services;
using Microsoft.IdentityModel.Tokens;

namespace Flatlinq.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthServices _authServices;
	private readonly ILogger<AuthController> _logger;
	public AuthController(IAuthServices authServices, ILogger<AuthController> logger)
	{
		_authServices = authServices;
		_logger = logger;
	}

	[HttpPost("Register")]
	public async Task<IActionResult> Register(RegisterDTO data)
	{
		try
		{
			TokenReturnDTO? tokens = await _authServices.RegisterUser(data);
			if (tokens is not null)
			{
				Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
				{
					HttpOnly = true,
					Secure = false,
					SameSite = SameSiteMode.Lax,
					Expires = DateTimeOffset.Now.AddDays(7)
				});
				Response.Headers.Add("authentication", tokens.AccessToken);
				return Ok("Successfully created user!");
			}
			return BadRequest("User already exists!");
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return BadRequest(e.Message);
		}
	}

	[HttpPost("Login")]
	public async Task<ActionResult> LoginUser(LoginDTO data)
	{
		TokenReturnDTO? tokens = await _authServices.LoginUser(data);
		if (tokens != null)
		{
			Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = false,
				SameSite = SameSiteMode.Lax,
				Expires = DateTimeOffset.Now.AddDays(7)
			});
			Response.Headers.Add("Authorization", tokens.AccessToken);
			return Ok("Successfully logged in user!");
		}
		return BadRequest("Email or password is wrong!");
	}

	[HttpPost("Facebook")]
	public async Task<ActionResult> FacebookAuthentication(string accessToken)
	{
		try
		{
			TokenReturnDTO tokens = await _authServices.ExternalAuthentication(accessToken, ExternalAuthenticationEnum.Facebook);
			Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = false,
				SameSite = SameSiteMode.Lax,
				Expires = DateTimeOffset.Now.AddDays(7)
			});
			Response.Headers.Add("Authorization", tokens.AccessToken);
			return Ok("Successfully logged in user!");
		}
		catch (MyException e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpPost("Google")]
	public async Task<ActionResult> GoogleAuthentication(string accessToken)
	{
		try
		{
			TokenReturnDTO tokens = await _authServices.ExternalAuthentication(accessToken, ExternalAuthenticationEnum.Google);
			Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = false,
				SameSite = SameSiteMode.Lax,
				Expires = DateTimeOffset.Now.AddDays(7)
			});
			Response.Headers.Add("Authorization", tokens.AccessToken);
			return Ok("Successfully logged in user!");
		}
		catch (MyException e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpGet("Verify")]
	public async Task<ActionResult> VerifyTokens()
	{
		try
		{
			string? refreshToken = Request.Cookies["refreshToken"];
			if (refreshToken == null)
			{
				return BadRequest("Not authenticated");
			}
			TokenReturnDTO tokens = await _authServices.VerifyTokens(refreshToken);
			Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = false,
				SameSite = SameSiteMode.Lax,
				Expires = DateTimeOffset.Now.AddDays(7)
			});
			Response.Headers.Add("Authorization", tokens.AccessToken);
			return Ok();
		}
		catch (SecurityTokenValidationException e)
		{
			return BadRequest(e.Message);
		}
	}

	[HttpPost("Role"), Authorize]
	public async Task<ActionResult> SetRole(RoleDTO data)
	{
		try
		{
			string role = data.IsTenant ? "Tenant" : "Landlord";
			await _authServices.SetRole(role, Request.Headers["Authorization"]);
			return Ok(string.Format("Successfully created {0} account for user!", role));
		}
		catch (MyException e)
		{
			return Ok(e.Message);
		}
		catch (Exception e)
		{
			_logger.LogError(new EventId(1), e, e.Message);
			return BadRequest("Somethiing has gone wrong, please check again later!");
		}
	}
}
