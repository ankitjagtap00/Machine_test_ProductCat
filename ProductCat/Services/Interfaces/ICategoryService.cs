using ProductCat.Helpers;
using ProductCat.Models;
using System.Collections;

namespace ProductCat.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedList<Category>> GetCategoriesAsync(int pageIndex = 1, int pageSize = 10);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<bool> IsCategoryNameUniqueAsync(string name, int? id = null);
        Task<ServiceResult> CreateCategoryAsync(Category category);
        Task<ServiceResult> UpdateCategoryAsync(Category category);
        Task<ServiceResult> DeleteCategoryAsync(int id);
        Task<IEnumerable> GetAllCategoriesAsync();
    }
}
