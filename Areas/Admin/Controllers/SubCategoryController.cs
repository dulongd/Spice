using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        [TempData] 
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //GET - INDEX
        public async Task<IActionResult> Index()
        {
            var subCategories = await _dbContext.SubCategories.Include(s => s.Category).ToListAsync();

            return View(subCategories);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _dbContext.Categories.ToListAsync(),
                SubCategory = new SubCategory(),
                SubcategoryList = await _dbContext.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()
            };
            return View(viewModel);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _dbContext.SubCategories.Include(s => s.Category)
                    .Where(s => s.Name == viewModel.SubCategory.Name &&
                                s.CategoryId == viewModel.SubCategory.CategoryId);

                if (doesSubCategoryExist.Count() > 0)
                {
                    //Error
                    StatusMessage =
                        $"Error: Subcategory exists under {doesSubCategoryExist.First().Category.Name}. Please use another name.";
                }
                else
                {
                    _dbContext.SubCategories.Add(viewModel.SubCategory);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            var model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _dbContext.Categories.ToListAsync(),
                SubCategory = viewModel.SubCategory,
                SubcategoryList = await _dbContext.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(model);

        }
    }
}
