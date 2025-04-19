using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug
            };
        }

        public async Task<int> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            var category = new DocumentCategory
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                Slug = categoryDto.Slug
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return category.Id;
        }

        public async Task UpdateCategoryAsync(CategoryDTO categoryDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
            if (category != null)
            {
                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;
                category.Slug = categoryDto.Slug;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category != null;
        }
    }
} 