using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; } 

        [Display(Name = "订单总额")]
        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateOnly PaymentDueDate { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        [Required]
        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "地址")]
        public string StreetAddress { get; set; }

        [Required]
        [Display(Name = "城市")]
        public string City { get; set; }

        [Required]
        [Display(Name = "省份")]
        public string State { get; set; }

        [Required]
        [Display(Name = "邮政编码")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "姓名")]
        public string Name { get; set; }
    }
}
