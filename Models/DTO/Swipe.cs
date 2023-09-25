using System.ComponentModel.DataAnnotations;

namespace Flatlinq.Models.DTO;

public class SwipeCardDTO
{
    [Required]
    [DataType(DataType.Text)]
    public string SwipedId { get; set; } = "";

    [DataType(DataType.Text)]
    public int? HouseId { get; set; }
}