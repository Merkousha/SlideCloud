using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.Interfaces;
using SlideCloud.Application.Models;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Application.Services;

public class SlideService : ISlideService
{
    private readonly ApplicationDbContext _context;

    public SlideService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Slide> GetSlideByIdAsync(int id)
    {
        return await _context.Slides
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Slide>> GetAllSlidesAsync()
    {
        return await _context.Slides
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<Slide> CreateSlideAsync(Slide slide)
    {
        _context.Slides.Add(slide);
        await _context.SaveChangesAsync();
        return slide;
    }

    public async Task<Slide> UpdateSlideAsync(Slide slide)
    {
        _context.Entry(slide).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return slide;
    }

    public async Task DeleteSlideAsync(int id)
    {
        var slide = await _context.Slides.FindAsync(id);
        if (slide != null)
        {
            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Slide>> GetSlidesByUserIdAsync(string userId)
    {
        return await _context.Slides
            .Include(s => s.User)
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }
} 