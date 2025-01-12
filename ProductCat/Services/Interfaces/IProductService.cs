using ProductCat.Helpers;
using ProductCat.Models;
using ProductCat.Models.ViewModels;

namespace ProductCat.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedList<ProductViewModel>> GetProductsAsync(int pageIndex = 1, int pageSize = 10);
        Task<Product> GetProductByIdAsync(int id);
        Task<bool> IsProductNameUniqueAsync(string name, int? id = null);
        Task<ServiceResult> CreateProductAsync(Product product);
        Task<ServiceResult> UpdateProductAsync(Product product);
        Task<ServiceResult> DeleteProductAsync(int id);
        
    }
}
