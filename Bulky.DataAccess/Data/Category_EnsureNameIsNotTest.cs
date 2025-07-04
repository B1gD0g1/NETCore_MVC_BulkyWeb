using Bulky.Models;
using System.ComponentModel.DataAnnotations;

namespace Bulky.DataAccess.Data
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
