using NETCore_MVC_BulkyWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace NETCore_MVC_BulkyWeb.Data
{
    public class Category_EnsureNameIsNotTest: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var category = validationContext.ObjectInstance as Category;

            if (category is not null && category.Name.Equals("test", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult("Test is an invalid value.");
            }

            return ValidationResult.Success;
        }
    }
}
