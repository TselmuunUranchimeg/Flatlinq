using Microsoft.AspNetCore.Identity;
using Flatlinq.Data;
using Flatlinq.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Flatlinq.Services;

public class SwipeServices : ISwipeServices
{
    private readonly IJwtServices _jwtServices;
    private readonly UserManager<User> _userManager;
    private readonly CoreDbContext _coreDbContext;

    public SwipeServices(IJwtServices jwtServices, 
        UserManager<User> userManager, CoreDbContext coreDbContext)
    {
        _jwtServices = jwtServices;
        _userManager = userManager;
        _coreDbContext = coreDbContext;
    }
    public async Task SwipeCard(string accessToken, SwipeCardDTO data)
    {
        string swiperId = _jwtServices.GetIdFromToken(accessToken);
        List<Task<User>> tasks = new()
        {
            _userManager.FindByIdAsync(swiperId)!,
            _userManager.FindByIdAsync(data.SwipedId)!
        };
        User[] users = await Task.WhenAll(tasks);
        UserSwipes? check = await _coreDbContext.Swipes!.FirstOrDefaultAsync(x => x.Swiped == users[1] && x.Swiper == users[0]);
        if (check != null)
        {
            //Send notifications to both users
        }
        await _coreDbContext.Swipes!.AddAsync(new UserSwipes
        {
            Swiped = users[1],
            Swiper = users[0],
            SwipedHouse = data.HouseId != null ? await _coreDbContext.Houses!.FirstOrDefaultAsync(x => x.Id == data.HouseId) : null
        });
        await _coreDbContext.SaveChangesAsync();
    }
}