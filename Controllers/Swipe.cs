using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flatlinq.Services;
using Flatlinq.Models.DTO;

namespace Flatlinq.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SwipeController: ControllerBase
{
    private readonly SwipeServices _swipeServices;

    public SwipeController(SwipeServices swipeServices)
    {
        _swipeServices = swipeServices;
    }
    [HttpPost]
    public async Task<ActionResult> Swipe(SwipeCardDTO data)
    {
        await _swipeServices.SwipeCard(Request.Headers.Authorization!, data);
        return Ok();
    }
}