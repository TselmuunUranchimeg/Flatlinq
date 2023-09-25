using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Amazon.Runtime;
using Flatlinq.Models.DTO;
using Flatlinq.Services;

[ApiController, Route("[controller]"), Authorize(Roles = "Landlord")]
public class HouseController : ControllerBase
{
    private readonly IHouseServices _houseServices;
    public HouseController(IHouseServices houseServices)
    {
        _houseServices = houseServices;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateHouse(CreateHouseDTO data)
    {
        try
        {
            await _houseServices.CreateHouse(data, Request.Headers.Authorization!);
            return Ok("Successfully created house!");
        }
        catch (AWSCommonRuntimeException e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest("Something went wrong, please check again later!");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetHouseById(int id)
    {
        try
        {
            string accessToken = Request.Headers.Authorization!;
            GetHouseResponseDTO data = await _houseServices.GetHouseById(accessToken, id);
            return Ok(data);
        }
        catch (MyException e)
        {
            return BadRequest(e.Message);
        }
    }
}
