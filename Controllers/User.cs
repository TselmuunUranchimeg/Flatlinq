using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flatlinq.Services;

namespace Flatlinq.Controllers;

[ApiController, Authorize, Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserServices _userServices;
    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }
    [HttpPost("Subscribe")]
    public async Task<ActionResult> SubscribeToGold()
    {
        try
        {
            string accessToken = Request.Headers.Authorization!;
            await _userServices.SubscribeToGold(accessToken);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }

    [HttpPost("BankId")]
    public async Task<ActionResult> VerifyBankId(string endUserIp)
    {
        string accessToken = Request.Headers.Authorization!;
        StartBankIdVerificationDTO data = await _userServices.StartBankIdVerification(endUserIp, accessToken);
        return Ok(data);
    }
}