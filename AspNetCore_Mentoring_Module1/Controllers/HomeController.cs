using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore_Mentoring_Module1.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCore_Mentoring_Module1.Models;
using AspNetCore_Mentoring_Module1.Models.OutputModels;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Mentoring_Module1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions _options;
        private readonly IProductModelProvider _modelProvider;
        private NorthwindContext _dbContext;

        private IEnumerable<Categories> _categories;
        private IEnumerable<Suppliers> _suppliers;

        private IEnumerable<Categories> CategoriesList => _categories ?? (_categories = _dbContext.Categories.ToList());
        private IEnumerable<Suppliers> SuppliersList => _suppliers ?? (_suppliers = _dbContext.Suppliers.ToList());

        public HomeController(ILogger<HomeController> logger, NorthwindContext context, IOptionsProvider optionsProvider, IProductModelProvider productModelProvider)
        {
            _logger = logger;
            _dbContext = context;
            _options = optionsProvider.GetOptions();
            _logger.LogInformation("Read configuration: product item to display:{0}", _options.NumberOfItemsForPaging);
            _modelProvider = productModelProvider;
            _categories = _dbContext.Categories.ToList();
            _suppliers = _dbContext.Suppliers.ToList();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Categories()
        {
            return View(CategoriesList);
        }

        public async Task<IActionResult> Products(int page = 1)
        {
            var count = await _dbContext.Products.CountAsync();
            var products = _options.NumberOfItemsForPaging == 0
                ? await _dbContext.Products.ToListAsync()
                : await _dbContext.Products.Skip((page - 1) * _options.NumberOfItemsForPaging).Take(_options.NumberOfItemsForPaging)
                    .ToListAsync();

            var preparedData = PrepareData(products);
            var pageViewModel = new PageViewModel(count, page, _options.NumberOfItemsForPaging);
            var viewModel = new ProductPagingViewModel {
                PageViewModel = pageViewModel,
                Products = preparedData.Result
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var productModel = _modelProvider.GetProducts(null);

            productModel.Categories = CategoriesList;
            productModel.Suppliers = SuppliersList;

            return View(productModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Products product)
        {
            //server side validation property level
            if (string.IsNullOrEmpty(product.QuantityPerUnit)) {
                ModelState.AddModelError("QuantityPerUnit", "QuantityPerUnit field cannot be empty");  
            }

            //server side validation model level
            if (product.UnitsInStock > 100) {
                ModelState.AddModelError("", "Unit in stock field cannot be more then 100");
            }

            if(ModelState.IsValid) {
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Products");
            } else {
                var model = _modelProvider.GetProducts(product);
                model.Categories = CategoriesList;
                model.Suppliers = SuppliersList;
            
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null)
                    return View(product);
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null) {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);

                if (product != null) {

                    var model = _modelProvider.GetProducts(product);
                    model.Categories = CategoriesList;
                    model.Suppliers = SuppliersList;

                    return View(model);
                }   
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Products product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Products");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null) {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null) {
                    var model = _modelProvider.GetProducts(product);
                    model.Categories = CategoriesList;
                    model.Suppliers = SuppliersList;
                    return View(model);
                }
            }
           
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null) {
                var product = new Products { ProductId = id.Value };

                _dbContext.Entry(product).State = EntityState.Deleted;

                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Products"); 
            }

            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<ProductModel>> PrepareData(List<Products> products)
        {
            var model = new List<ProductModel>();

            await Task.Run(() => products.ForEach(item => {
                var supplierName = _dbContext.Suppliers.FirstOrDefault(s => s.SupplierId == item.SupplierId)?.CompanyName;
                var categoryName = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == item.CategoryId)?.CategoryName;

                var productModel = _modelProvider.GetProducts(item);
                productModel.SupplierName = supplierName;
                productModel.CategoryName = categoryName;
                model.Add(productModel);
            }));

            return model;
        }
    }
}
