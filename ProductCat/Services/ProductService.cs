using Microsoft.EntityFrameworkCore;
using ProductCat.Data;
using ProductCat.Helpers;
using ProductCat.Models;
using ProductCat.Models.ViewModels;
using ProductCat.Services.Interfaces;

namespace ProductCat.Services
{

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<ProductViewModel>> GetProductsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var productsQuery = from p in _context.Products
                                join c in _context.Categories on p.CategoryId equals c.CategoryId
                                select new ProductViewModel
                                {
                                    ProductId = p.ProductId,
                                    ProductName = p.ProductName,
                                    CategoryId = p.CategoryId,
                                    CategoryName = c.CategoryName
                                };

            return await PaginatedList<ProductViewModel>.CreateAsync(
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

        public async Task<bool> IsProductNameUniqueAsync(string name, int? id = null)
        {
            return !await _context.Products.AnyAsync(p =>
                p.ProductName.ToLower() == name.ToLower() &&
                (!id.HasValue || p.ProductId != id.Value));
        }

        public async Task<ServiceResult> CreateProductAsync(Product product)
        {
            try
            {
                if (!await IsProductNameUniqueAsync(product.ProductName))
                {
                    return ServiceResult.Fail("A product with this name already exists.");
                }

                if (!await _context.Categories.AnyAsync(c => c.CategoryId == product.CategoryId))
                {
                    return ServiceResult.Fail("Selected category does not exist.");
                }

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error creating product: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateProductAsync(Product product)
        {
            try
            {
                if (!await IsProductNameUniqueAsync(product.ProductName, product.ProductId))
                {
                    return ServiceResult.Fail("A product with this name already exists.");
                }

                if (!await _context.Categories.AnyAsync(c => c.CategoryId == product.CategoryId))
                {
                    return ServiceResult.Fail("Selected category does not exist.");
                }

                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error updating product: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return ServiceResult.Fail("Product not found.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Error deleting product: {ex.Message}");
            }
        }
    }

}
