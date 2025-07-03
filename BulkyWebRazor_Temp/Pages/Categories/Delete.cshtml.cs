using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        [BindProperty]
        public Category Category { get; set; }

        public DeleteModel(ApplicationDbContext dbContext)
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
            Category categoryToDelete = dbContext.Categories.Find(Category.Id);
            if (categoryToDelete is null)
            {
                return NotFound();
            }

            dbContext.Categories.Remove(categoryToDelete);
            await dbContext.SaveChangesAsync();

            TempData["success"] = "类别删除成功！";

            return RedirectToPage("Index", "Catogries");
        }
    }
}
