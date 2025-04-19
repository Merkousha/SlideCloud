using SlideCloud.Application.Models;

namespace SlideCloud.Application.Interfaces;

public interface ISlideService
{
    Task<Slide> GetSlideByIdAsync(int id);
    Task<IEnumerable<Slide>> GetAllSlidesAsync();
    Task<Slide> CreateSlideAsync(Slide slide);
    Task<Slide> UpdateSlideAsync(Slide slide);
    Task DeleteSlideAsync(int id);
    Task<IEnumerable<Slide>> GetSlidesByUserIdAsync(string userId);
} 