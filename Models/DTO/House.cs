using System.ComponentModel.DataAnnotations;

namespace Flatlinq.Models.DTO;

public class CreateHouseDTO
{
    [Required]
    public int Price { get; set; } = 0;
	[Required]
    public string Name { get; set; } = "";
	[Required]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = "";
	[Required]
    public bool HasInternet { get; set; } = false;
	[Required]
    public bool HasElectricity { get; set; } = false;
	[Required]
    public bool AllowChildren { get; set; } = false;
	[Required]
    public bool AllowPets { get; set; } = false;
	[Required]
    public bool AllowSmoking { get; set; } = false;
    [Required]
    public List<IFormFile> Files { get; set; } = new();
}

public class GetHouseResponseDTO
{
    public int Price { get; set; } = 0;
	public string Name { get; set; } = "";
	public string Description { get; set; } = "";
	public bool HasInternet { get; set; } = false;
	public bool HasElectricity { get; set; } = false;
	public bool AllowChildren { get; set; } = false;
	public bool AllowPets { get; set; } = false;
	public bool AllowSmoking { get; set; } = false;
    public string[] Images { get; set; } = new string[]{};
}