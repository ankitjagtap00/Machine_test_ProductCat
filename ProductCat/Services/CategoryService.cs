using Microsoft.EntityFrameworkCore;
using ProductCat.Data;
using ProductCat.Helpers;
using ProductCat.Models;
using ProductCat.Services.Interfaces;
using System.Collections;

namespace ProductCat.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Category>> GetCategoriesAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Categories.OrderBy(c => c.CategoryName);
            return await PaginatedList<Category>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? id = null)
        {
            return !await _context.Categories.AnyAsync(c =>
                c.CategoryName.ToLower() == name.ToLower() &&
                (!id.HasValue || c.CategoryId != id.Value));
        }

        public async Task<ServiceResult> CreateCategoryAsync(Category category)
        {
            try
            {
                if (!await IsCategoryNameUniqueAsync(category.CategoryName))
                {
                    return ServiceResult.Fail("A category with this name already exists.");
                }

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error creating category: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateCategoryAsync(Category category)
        {
            try
            {
                if (!await IsCategoryNameUniqueAsync(category.CategoryName, category.CategoryId))
                {
                    return ServiceResult.Fail("A category with this name already exists.");
                }

                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error updating category: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return ServiceResult.Fail("Category not found.");
                }

                if (await _context.Products.AnyAsync(p => p.CategoryId == id))
                {
                    return ServiceResult.Fail("Cannot delete category that has associated products.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error deleting category: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

       
    }


}
