using Flatlinq.Models.DTO;

namespace Flatlinq.Services;

public interface ISwipeServices
{
    Task SwipeCard(string accessToken, SwipeCardDTO data);
}