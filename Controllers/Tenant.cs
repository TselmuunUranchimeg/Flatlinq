using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Flatlinq.Services;
using Flatlinq.Models.DTO;

namespace Flatlinq.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Tenant")]
public class TenantController: ControllerBase
{
	private readonly ITenantServices _tenantServices;
	public TenantController(ITenantServices tenantServices)
	{
		_tenantServices = tenantServices;
	}

	[HttpGet("Recommendation")]
	public IActionResult Get(
		[FromQuery] bool? hasElectricy,
		[FromQuery] bool? hasInternet,
		[FromQuery] bool? allowChildren,
		[FromQuery] bool? allowPets,
		[FromQuery] bool? allowSmoking,
		[FromQuery] int? minPrice,
		[FromQuery] int? maxPrice,
		[FromQuery] int position
	)
	{
		IQueryable<House> query = _tenantServices.GetRecommendation(new GetRecommendationDTO
		{
			HasElectricity = hasElectricy,
			HasInternet = hasInternet,
			AllowChildren = allowChildren,
			AllowPets = allowPets,
			AllowSmoking = allowSmoking,
			MinPrice = minPrice,
			MaxPrice = maxPrice
		}, position);
		return Ok(query);
	}
}
