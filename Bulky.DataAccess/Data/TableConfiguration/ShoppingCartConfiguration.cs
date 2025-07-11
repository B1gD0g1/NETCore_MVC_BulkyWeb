using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Data.TableConfiguration
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            //builder.HasKey(s => s.Id);

            //builder.HasOne(s => s.Product)
            //    .WithMany()
            //    .HasForeignKey(s => s.ProductId);

            //builder.HasOne(s => s.ApplicationUser)
            //    .WithMany()
            //    .HasForeignKey(s => s.ApplicationUserId);
        }
    }
}
