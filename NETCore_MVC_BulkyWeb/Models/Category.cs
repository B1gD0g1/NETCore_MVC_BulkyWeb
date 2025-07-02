using NETCore_MVC_BulkyWeb.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NETCore_MVC_BulkyWeb.Models
{
    public class Category: IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("类别名称")]
        [Required(ErrorMessage = "类别名称不能为空。")]
        [MaxLength(30,ErrorMessage = "类别名称最大为30字符。")]
        public string Name { get; set; }

        [DisplayName("显示顺序")]
        //[Range(1,100, ErrorMessage = "该字段必须在1~100之间。")]
        public int DisplayOrder { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name is not null && Name.Equals("test", StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("Test是无效值。", new[] {""});
            }

            if (DisplayOrder < 1 || DisplayOrder > 100)
            {
                yield return new ValidationResult("显示顺序必须在1-100之间", new[] { nameof(DisplayOrder) });
            }
        }
    }
}
