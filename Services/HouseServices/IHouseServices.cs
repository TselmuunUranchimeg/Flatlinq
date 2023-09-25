using Flatlinq.Models.DTO;

namespace Flatlinq.Services;

public interface IHouseServices
{
    Task CreateHouse(CreateHouseDTO data, string accessToken);
    Task<GetHouseResponseDTO> GetHouseById(string accessToken, int houseId);
}