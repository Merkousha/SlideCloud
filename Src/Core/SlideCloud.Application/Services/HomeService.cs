using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.DTO.Home;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HomeDTO> LoadHomeDataAsync()
        {
            var newUsers = await _unitOfWork.Users.GetAll()
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            var latestDocuments = await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .ToListAsync();

            var popularDocuments = await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .OrderByDescending(d => d.ViewCount)
                .Take(5)
                .ToListAsync();

            var newCategories = await _unitOfWork.Categories.GetAll()
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToListAsync();

            return new HomeDTO
            {
                NewUsers = newUsers,
                LatestDocuments = latestDocuments,
                PopularDocuments = popularDocuments,
                NewCategories = newCategories
            };
        }

        public async Task<HomeViewModel> GetHomeViewModelAsync(long userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return null;

            var recentFiles = await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .ToListAsync();

            var sharedDocuments = await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Where(d => d.IsShared)
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .ToListAsync();

            return new HomeViewModel
            {
                UserId = userId,
                UserName = user.Name,
                RecentFiles = recentFiles,
                SharedDocuments = sharedDocuments
            };
        }

        public async Task<IEnumerable<Document>> GetLatestDocumentsAsync()
        {
            return await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.User)
                .OrderByDescending(d => d.CreatedAt)
                .Take(6)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetPopularDocumentsAsync()
        {
            return await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.User)
                .OrderByDescending(d => d.ViewCount)
                .Take(6)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetUserDocumentsAsync(string userId)
        {
            return await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.User)
                .Where(d => d.UserId.ToString() == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Take(6)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetSharedDocumentsAsync()
        {
            return await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.User)
                .Where(d => d.IsShared)
                .OrderByDescending(d => d.CreatedAt)
                .Take(6)
                .ToListAsync();
        }
    }
}