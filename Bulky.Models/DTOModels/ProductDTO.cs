using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.DTOModels
{
    //只包含前端需要的字段）
    public class ProductDTO
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "标题不能为空。")]
        public string Title { get; set; }

        public string Description { get; set; } // 可选：列表页通常不需要描述

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Author { get; set; }

        [Range(1, 1000)]
        public double ListPrice { get; set; }

        // 分类简略信息（避免循环引用）
        public CategoryDTO? Category { get; set; }

        // 图片URL（前端可能需要显示缩略图）
        public string ImageUrl { get; set; }
    }
}
