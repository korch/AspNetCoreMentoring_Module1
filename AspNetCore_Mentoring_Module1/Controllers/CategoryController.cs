using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Mentoring_Module1.Controllers
{
    public class CategoryController : Controller
    {
        private NorthwindContext _dbContext;

        private IEnumerable<Categories> _categories;
        private IEnumerable<Categories> CategoriesList => _categories ??= _dbContext.Categories.ToList();
        public CategoryController(NorthwindContext context)
        {
            _dbContext = context;
        }

        public async Task<IActionResult> Category()
        {
            return View(CategoriesList);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null) {
                var category = await _dbContext.Categories.FirstOrDefaultAsync(p => p.CategoryId == id);

                if (category != null) {
                    return View(category);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Categories category, List<IFormFile> Picture)
        {
            foreach (var item in Picture) {
                if (item.Length > 0) {
                    category.Picture = ConvertToBytes(item);
                }

                _dbContext.Categories.Update(category);

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Category");
        }
        
        public async Task<IActionResult> ViewImage(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _dbContext.Categories.FirstOrDefaultAsync(p => p.CategoryId == id);
            var picture = category.Picture;

            ViewBag.Id = id;

            return File(picture, "image/png");
        }

        private byte[] ConvertToBytes(IFormFile image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)image.Length);
            return CoverImageBytes;
        }
    }
}