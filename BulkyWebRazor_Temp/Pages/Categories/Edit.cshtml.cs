using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        [BindProperty]
        public Category Category { get; set; }

        public EditModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet(int? id)
        {
            if (id is not null)
            {
                Category = dbContext.Categories.Find(id);
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                dbContext.Categories.Update(Category);
                await dbContext.SaveChangesAsync();

                TempData["success"] = "类别更新成功！";

                return RedirectToPage("Index", "Categories");
            }

            return Page();
        }
    }
}
