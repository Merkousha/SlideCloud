using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Application.Services
{
    public class SlideService : ISlideService
    {
        private readonly ISlideRepository _slideRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public SlideService(
            ISlideRepository slideRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository)
        {
            _slideRepository = slideRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public async Task<PaginatedList<SlideDTO>> GetAllSlidesAsync(int pageIndex = 1, int? categoryId = null, string searchTerm = null, int pageSize = 12)
        {
            var query = _slideRepository.GetAll()
                .Include(s => s.DocumentCategory)
                .Include(s => s.DocumentType)
                .Include(s => s.User)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(s => s.DocumentCategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(s => 
                    s.Title.ToLower().Contains(searchTerm) || 
                    s.Description.ToLower().Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SlideDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    FilePath = s.FilePath,
                    ThumbnailPath = s.ThumbnailPath,
                    DocumentCategoryId = s.DocumentCategoryId,
                    DocumentCategoryName = s.DocumentCategory.Name,
                    DocumentTypeId = s.DocumentTypeId,
                    DocumentTypeName = s.DocumentType.Name,
                    UserId = s.UserId,
                    UserName = s.User.UserName,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return new PaginatedList<SlideDTO>(items, totalItems, pageIndex, pageSize);
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAll()
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return categories;
        }

        public async Task<SlideDTO> GetSlideDetailAsync(int id)
        {
            var slide = await _slideRepository.GetAll()
                .Include(s => s.DocumentCategory)
                .Include(s => s.DocumentType)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slide == null)
                return null;

            return new SlideDTO
            {
                Id = slide.Id,
                Title = slide.Title,
                Description = slide.Description,
                FilePath = slide.FilePath,
                ThumbnailPath = slide.ThumbnailPath,
                DocumentCategoryId = slide.DocumentCategoryId,
                DocumentCategoryName = slide.DocumentCategory.Name,
                DocumentTypeId = slide.DocumentTypeId,
                DocumentTypeName = slide.DocumentType.Name,
                UserId = slide.UserId,
                UserName = slide.User.UserName,
                CreatedAt = slide.CreatedAt
            };
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var slide = await _slideRepository.GetByIdAsync(id);
            if (slide != null)
            {
                slide.ViewCount++;
                await _slideRepository.UpdateAsync(slide);
            }
        }

        public async Task<string> GetAuthorNameAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.UserName;
        }

        public async Task<IEnumerable<SlideDTO>> GetAuthorSlidesAsync(string userId)
        {
            var slides = await _slideRepository.GetAll()
                .Include(s => s.DocumentCategory)
                .Include(s => s.DocumentType)
                .Include(s => s.User)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SlideDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    FilePath = s.FilePath,
                    ThumbnailPath = s.ThumbnailPath,
                    DocumentCategoryId = s.DocumentCategoryId,
                    DocumentCategoryName = s.DocumentCategory.Name,
                    DocumentTypeId = s.DocumentTypeId,
                    DocumentTypeName = s.DocumentType.Name,
                    UserId = s.UserId,
                    UserName = s.User.UserName,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return slides;
        }

        public async Task<PaginatedList<SlideDTO>> GetUserSlidesAsync(string userId, int pageIndex = 1, int pageSize = 12)
        {
            var query = _slideRepository.GetAll()
                .Include(s => s.DocumentCategory)
                .Include(s => s.DocumentType)
                .Include(s => s.User)
                .Where(s => s.UserId == userId);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SlideDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    FilePath = s.FilePath,
                    ThumbnailPath = s.ThumbnailPath,
                    DocumentCategoryId = s.DocumentCategoryId,
                    DocumentCategoryName = s.DocumentCategory.Name,
                    DocumentTypeId = s.DocumentTypeId,
                    DocumentTypeName = s.DocumentType.Name,
                    UserId = s.UserId,
                    UserName = s.User.UserName,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return new PaginatedList<SlideDTO>(items, totalItems, pageIndex, pageSize);
        }

        public async Task<Stream> ConvertSlideToImageAsync(string filePath)
        {
            // این متد باید پیاده‌سازی شود تا فایل اسلاید را به تصویر تبدیل کند
            throw new NotImplementedException();
        }
    }
} 