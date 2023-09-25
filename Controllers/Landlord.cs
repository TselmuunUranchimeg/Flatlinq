using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flatlinq.Services;

namespace Flatlinq.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Landlord")]
public class LandlordConroller: ControllerBase
{
    private readonly ILandlordServices _landlordServices;
    public LandlordConroller(ILandlordServices landlordServices)
    {
        _landlordServices = landlordServices;
    }
    [HttpGet("Recommendation")]
    public ActionResult GetRecommendation([FromQuery] int position)
    {
        ServiceDTO data = _landlordServices.GetRecommendation(Request.Headers["Authorizaiton"]!, position);
        return Ok(data);
    }
}