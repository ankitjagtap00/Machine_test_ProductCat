using Microsoft.EntityFrameworkCore;
using ProductCat.Data;
using ProductCat.Helpers;
using ProductCat.Models;
using ProductCat.Models.ViewModels;
using ProductCat.Services.Interfaces;
using ProductCat.ViewModels;

namespace ProductCat.Services
{

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Models.ViewModels.ProductViewModel>> GetProductsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var productsQuery = from p in _context.Products
                                join c in _context.Categories on p.CategoryId equals c.CategoryId
                                select new Models.ViewModels.ProductViewModel
                                {
                                    ProductId = p.ProductId,
                                    ProductName = p.ProductName,
                                    CategoryId = p.CategoryId,
                                    
                                };

            return await PaginatedList<Models.ViewModels.ProductViewModel>.CreateAsync(
                productsQuery.AsNoTracking(),
                pageIndex,
                pageSize);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public ServiceResult CreateProduct(Product product)
        {
            try
            {
                if (ProductExists(product.ProductName))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "A product with this name already exists."
                    };
                }

                if (!_context.Categories.Any(c => c.CategoryId == product.CategoryId))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Selected category does not exist."
                    };
                }

                _context.Products.Add(product);
                _context.SaveChanges();

                return new ServiceResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error creating product: {ex.Message}"
                };
            }
        }

        public ServiceResult UpdateProduct(Product product)
        {
            try
            {
                if (ProductExists(product.ProductName, product.ProductId))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "A product with this name already exists."
                    };
                }

                if (!_context.Categories.Any(c => c.CategoryId == product.CategoryId))
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Selected category does not exist."
                    };
                }

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return new ServiceResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error updating product: {ex.Message}"
                };
            }
        }

        public ServiceResult DeleteProduct(int id)
        {
            try
            {
                var product = _context.Products.Find(id);

                if (product == null)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Product not found."
                    };
                }

                _context.Products.Remove(product);
                _context.SaveChanges();

                return new ServiceResult { Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error deleting product: {ex.Message}"
                };
            }
        }

        public bool ProductExists(string name, int? excludeId = null)
        {
            return _context.Products.Any(p =>
                p.ProductName.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || p.ProductId != excludeId.Value));
        }

        

        

        public Task<bool> IsProductNameUniqueAsync(string name, int? id = null)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> CreateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<PaginatedList<Models.ViewModels.ProductViewModel>> IProductService.GetProductsAsync(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }

}
