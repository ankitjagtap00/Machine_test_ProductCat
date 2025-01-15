using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductCat.Models;
using ProductCat.Models.ViewModels;
using ProductCat.Services.Interfaces;

namespace ProductCat.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10;
            int pageIndex = page ?? 1;
            var products = await _productService.GetProductsAsync(pageIndex, pageSize);
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["Categories"] = new SelectList(categories, "CategoryId", "CategoryName");
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var isNameUnique = await _productService.IsProductNameUniqueAsync(productViewModel.ProductName);
                if (!isNameUnique)
                {
                    ModelState.AddModelError("ProductName", "A product with this name already exists.");
                    var categoriesList = await _categoryService.GetAllCategoriesAsync();
                    ViewData["Categories"] = new SelectList(categoriesList, "CategoryId", "CategoryName");
                    return View(productViewModel);
                }

                var product = new Product
                {
                    ProductName = productViewModel.ProductName,
                    CategoryId = productViewModel.CategoryId
                };

                var result = await _productService.CreateProductAsync(product);
                if (result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["Categories"] = new SelectList(categories, "CategoryId", "CategoryName");
            return View(productViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId
            };

            await PopulateCategoriesDropDown(viewModel.CategoryId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel productViewModel)
        {
            if (id != productViewModel.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var isNameUnique = await _productService.IsProductNameUniqueAsync(productViewModel.ProductName, productViewModel.ProductId);
                if (!isNameUnique)
                {
                    ModelState.AddModelError("ProductName", "A product with this name already exists.");
                    await PopulateCategoriesDropDown(productViewModel.CategoryId);
                    return View(productViewModel);
                }

                var product = new Product
                {
                    ProductId = productViewModel.ProductId,
                    ProductName = productViewModel.ProductName,
                    CategoryId = productViewModel.CategoryId
                };

                var result = await _productService.UpdateProductAsync(product);
                if (result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            await PopulateCategoriesDropDown(productViewModel.CategoryId);
            return View(productViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", result.Message);

            // Reload the product for the view if deletion fails
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                
            };

            return View(viewModel);
        }

        private async Task PopulateCategoriesDropDown(int? selectedCategoryId = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(
                categories,
                "CategoryId",
                "CategoryName",
                selectedCategoryId
            );
        }
    }
}
