namespace Flatlinq.Models;

public class Channel
{
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public ICollection<Message> Messages { get; set; } = null!;
	public ICollection<User> Members { get; set; } = null!;
}

public class Message
{
	public int Id { get; set; }
	public int ChannelId { get; set; }
	public Channel Channel { get; set; } = null!;
	public string SenderId { get; set; } = "";
	public User Sender { get; set; } = null!;
	public string Text { get; set; } = "";
}
