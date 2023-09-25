using Flatlinq.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Flatlinq.Hubs;

public class ChatHub : Hub
{
    private readonly CoreDbContext _coreDbContext;
    private readonly UserManager<User> _userManager;

    public ChatHub(CoreDbContext coreDbContext, UserManager<User> userManager)
    {
        _coreDbContext = coreDbContext;
        _userManager = userManager;
    }
    public async Task JoinGroup(string groupName)
    {
        await _coreDbContext.Channels!.AddAsync(new Channel
        {
            Name = groupName
        });
        await _coreDbContext.SaveChangesAsync();
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendMessage(string groupName, string message, string senderId)
    {
        User sender = (await _userManager.FindByIdAsync(senderId))!;
        Channel channel = (await _coreDbContext.Channels!
            .FirstOrDefaultAsync(x => x.Name == groupName))!;
        await _coreDbContext.Messages!.AddAsync(new Message
        {
            Text = message,
            Sender = sender,
            Channel = channel
        });
        await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
    }
}