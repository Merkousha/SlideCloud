using SlideCloud.Application.DTO.Home;

namespace SlideCloud.Application.Interfaces
{
    public interface IHomeService
    {
        Task<HomeDTO> LoadHomeDataAsync();
        Task<HomeViewModel> GetHomeViewModelAsync(long userId);
    }
} 