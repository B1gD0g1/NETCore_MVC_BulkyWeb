using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "公司名称")]
        public string Name { get; set; }

        [Display(Name = "地址")]
        public string? StreetAddress { get; set; }

        [Display(Name = "城市")]
        public string? City { get; set; }

        [Display(Name = "省份")]
        public string? State { get; set; }

        [Display(Name = "邮政编码")]
        public string? PostalCode { get; set; }

        [Display(Name = "电话号码")]
        public string? PhoneNumber { get; set; }
    }
}
