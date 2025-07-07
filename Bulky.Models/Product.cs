using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "标题不能为空。")]
        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "简介")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "国际标准图书编号")]
        public string ISBN { get; set; }

        [Required]
        [Display(Name = "作者")]
        public string Author { get; set; }

        [Required(ErrorMessage = "价格不能为空。")]
        [Display(Name = "价格")]
        [Range(1, 1000, ErrorMessage = "价格必须在1到1000之间。")]
        public Double ListPrice { get; set; }

        [Required(ErrorMessage = "价格不能为空。")]
        [Display(Name = "1~50的价格")]
        [Range(1, 1000, ErrorMessage = "价格必须在1到1000之间。")]
        public Double Price { get; set; }

        [Required(ErrorMessage = "价格不能为空。")]
        [Display(Name = "50+的价格")]
        [Range(1, 1000, ErrorMessage = "价格必须在1到1000之间。")]
        public Double Price50 { get; set; }

        [Required(ErrorMessage = "价格不能为空。")]
        [Display(Name = "100+的价格")]
        [Range(1, 1000, ErrorMessage = "价格必须在1到1000之间。")]
        public Double Price100 { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

        public int CategoryId { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
