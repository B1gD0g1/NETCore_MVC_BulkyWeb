using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.DTOModels
{
    // 分类简略信息DTO（不包含产品列表）
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "类别名称不能为空。")]
        [MaxLength(30, ErrorMessage = "类别名称最大为30字符。")]
        public string Name { get; set; }

        //[Range(1,100, ErrorMessage = "该字段必须在1~100之间。")]
        public int DisplayOrder { get; set; }
    }
}
