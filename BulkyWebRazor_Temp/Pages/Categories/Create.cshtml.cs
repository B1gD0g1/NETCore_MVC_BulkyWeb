using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        [BindProperty]
        public Category Category { get; set; }

        public CreateModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            dbContext.Categories.Add(Category);
            await dbContext.SaveChangesAsync();

            TempData["success"] = "类别创建成功！";
            return RedirectToPage("Index", "Categories");
        }
    }
}
