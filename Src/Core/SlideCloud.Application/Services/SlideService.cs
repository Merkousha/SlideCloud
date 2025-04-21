using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.DTO.DocumentType;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Models.Pagination;

namespace SlideCloud.Application.Services
{
    public class SlideService : ISlideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public SlideService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<PaginationModel<Document>> GetSlidesAsync(int pageIndex, int? categoryId, string searchTerm)
        {
            int pageSize = 12;
            var query = _unitOfWork.Documents.GetAll()
                 .Include(d => d.DocumentCategory)
                 .Include(d => d.DocumentType)
                 .Include(d => d.User)
                 .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(d => d.DocumentCategoryId == categoryId.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationModel<Document>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

        public async Task<DetailsSlideDTO> GetSlideByIdAsync(int id)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            if (document == null)
                return null;

            return new DetailsSlideDTO
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                File = document.File,
                Picture = document.Picture,
                DocumentTypeId = document.DocumentTypeId,
                DocumentCategoryId = document.DocumentCategoryId,
                ViewCount = document.ViewCount,
                CreatedAt = document.CreatedAt,
                UserId = document.UserId.ToString(),
                UserName = document.User?.UserName,
                IsShared = document.IsShared,
                DocumentType = new DocumentTypeDTO
                {
                    Id = document.DocumentType.Id,
                    Name = document.DocumentType.Name,
                    Description = document.DocumentType.Description
                },
                DocumentCategory = new CategoryDTO
                {
                    Id = document.DocumentCategory.Id,
                    Name = document.DocumentCategory.Name,
                    Description = document.DocumentCategory.Description
                }
            };
        }

        public async Task<DetailsSlideDTO> GetSlideDetailAsync(int id)
        {
            var document = await GetSlideByIdAsync(id);
            if (document != null)
            {
                await IncrementViewCountAsync(id);
            }
            return document;
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            if (document != null)
            {
                document.ViewCount++;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<PaginationModel<Document>> GetUserSlidesAsync(string userId, int pageIndex, int pageSize)
        {
            var query = _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.DocumentType)
                .Include(d => d.User)
                .Where(d => d.UserId.ToString() == userId)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationModel<Document>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

        public async Task<string> GetAuthorNameAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(long.Parse(userId));
            return user?.UserName ?? string.Empty;
        }

        public async Task<IEnumerable<Document>> GetAuthorSlidesAsync(string userId)
        {
            return await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.DocumentType)
                .Where(d => d.UserId.ToString() == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<byte[]> ConvertSlideToImageAsync(string filePath)
        {
            // TODO: Implement slide to image conversion
            throw new NotImplementedException();
        }

        public async Task CreateSlideAsync(SlideCreateDTO slideDto)
        {
            var document = new Document
            {
                Title = slideDto.Title,
                Description = slideDto.Description,
                File = slideDto.File,
                Picture = slideDto.Picture,
                DocumentTypeId = slideDto.DocumentTypeId,
                DocumentCategoryId = slideDto.DocumentCategoryId,
                UserId = long.Parse(slideDto.UserId),
                CreatedAt = DateTime.UtcNow,
                ViewCount = 0,
                IsShared = false
            };

            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateSlideAsync(SlideUpdateDTO slideDto)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(slideDto.Id);
            if (document == null)
                return;

            document.Title = slideDto.Title;
            document.Description = slideDto.Description;
            document.File = slideDto.File;
            document.Picture = slideDto.Picture;
            document.DocumentTypeId = slideDto.DocumentTypeId;
            document.DocumentCategoryId = slideDto.DocumentCategoryId;

            _unitOfWork.Documents.Update(document);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSlideAsync(int id)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            if (document != null)
            {
                if (document.File != null)
                {
                    await _fileService.DeleteFileAsync(document.File);
                }
                if (document.Picture != null)
                {
                    await _fileService.DeleteFileAsync(document.Picture);
                }
                _unitOfWork.Documents.Delete(document);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAll().ToListAsync();
            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Slug = c.Slug
            });
        }

        public async Task<IEnumerable<DocumentTypeDTO>> GetAllDocumentTypesAsync()
        {
            var types = await _unitOfWork.DocumentTypes.GetAll().ToListAsync();
            return types.Select(t => new DocumentTypeDTO
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Slug = t.Slug
            });
        }
    }
}