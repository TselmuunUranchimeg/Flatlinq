namespace Flatlinq.Models;

public class UserSwipes
{
	public int Id { get; set; } = 0;
	public string SwiperId { get; set; } = "";
	public string SwipedId { get; set; } = "";
	public int? HouseId { get; set; }
	public User Swiper { get; set; } = null!;
	public User Swiped { get; set; } = null!;
	public House? SwipedHouse { get; set; }
}
