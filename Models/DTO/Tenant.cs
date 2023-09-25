namespace Flatlinq.Models.DTO;

public class GetRecommendationDTO
{
    public bool? HasInternet { get; set; }
    public bool? HasElectricity { get; set; }
	public bool? AllowChildren { get; set; }
	public bool? AllowPets { get; set; }
	public bool? AllowSmoking { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
}

public class GetRecommendationReturnDTO
{
    public int LandlordId { get; set; }
    public int HouseId { get; set; }
    public string HouseName { get; set; } = "";
    public string HouseDescription { get; set; } = "";
    public string HousePrice { get; set; } = "";
    public bool HasInternet { get; set; }
	public bool HasElectricity { get; set; }
	public bool AllowChildren { get; set; }
	public bool AllowPets { get; set; }
	public bool AllowSmoking { get; set; }
    public string[] ImageURLs { get; set; } = new string[]{};
}