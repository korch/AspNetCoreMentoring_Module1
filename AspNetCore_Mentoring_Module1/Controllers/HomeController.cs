﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCore_Mentoring_Module1.Models;
using AspNetCore_Mentoring_Module1.Models.OutputModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspNetCore_Mentoring_Module1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private int _numberOfItemsOnPage = 0;

        private NorthwindContext _dbContext;

        public HomeController(ILogger<HomeController> logger, NorthwindContext context, IConfiguration config)
        {
            _logger = logger;
            _dbContext = context;
            _configuration = config;

            GetNumberOfItemsOnPage();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Categories()
        {
            return View(await _dbContext.Categories.ToListAsync());
        }

        public async Task<IActionResult> Products(int page = 1)
        {
            var count = await _dbContext.Products.CountAsync();
            var products = _numberOfItemsOnPage == 0
                ? await _dbContext.Products.ToListAsync()
                : await _dbContext.Products.Skip((page - 1) * _numberOfItemsOnPage).Take(_numberOfItemsOnPage)
                    .ToListAsync();

            var preparedData = PrepareData(products);
            var pageViewModel = new PageViewModel(count, page, _numberOfItemsOnPage);
            var viewModel = new ProductPagingViewModel {
                PageViewModel = pageViewModel,
                Products = preparedData.Result
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var productModel = new ProductModel() { 
                Categories = await _dbContext.Categories.ToListAsync(),
                Suppliers = await _dbContext.Suppliers.ToListAsync()
            };

            return View(productModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Products product)
        {
            if(ModelState.IsValid) {
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Products");
            }

            return View(product);
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
                    var model = new ProductModel {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        SupplierId = product.SupplierId,
                        CategoryId = product.CategoryId,
                        QuantityPerUnit = product.QuantityPerUnit,
                        UnitPrice = product.UnitPrice,
                        UnitsInStock = product.UnitsInStock,
                        UnitsOnOrder = product.UnitsOnOrder,
                        ReorderLevel = product.ReorderLevel,
                        Discontinued = product.Discontinued,
                        Categories = await _dbContext.Categories.ToListAsync(),
                        Suppliers = await _dbContext.Suppliers.ToListAsync()
                    };

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
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null)
                    return View(product);
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

                model.Add(new ProductModel {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    SupplierName = supplierName,
                    CategoryName = categoryName,
                    QuantityPerUnit = item.QuantityPerUnit,
                    UnitPrice = item.UnitPrice,
                    UnitsInStock = item.UnitsInStock,
                    UnitsOnOrder = item.UnitsOnOrder,
                    ReorderLevel = item.ReorderLevel,
                    Discontinued = item.Discontinued
                });
            }));

            return model;
        }

        private void GetNumberOfItemsOnPage()
        {
            _numberOfItemsOnPage = int.Parse(_configuration.GetSection("M-Products").GetSection("Count").Value);
        }
    }
}
