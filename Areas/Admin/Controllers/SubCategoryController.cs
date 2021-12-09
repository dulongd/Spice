using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            var subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _dbContext.SubCategories
                where subCategory.CategoryId == id
                select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _dbContext.SubCategories.SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _dbContext.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubcategoryList = await _dbContext.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()
            };

            return View(viewModel);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel viewModel)
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
                    var subCategoryInDb = await _dbContext.SubCategories.FindAsync(viewModel.SubCategory.Id);
                    subCategoryInDb.Name = viewModel.SubCategory.Name;
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

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _dbContext.SubCategories.Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);

        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _dbContext.SubCategories.Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //POST - DELETE

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _dbContext.SubCategories.FindAsync(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            _dbContext.SubCategories.Remove(subCategory);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

    }
}
