using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Product productObj)
        {
            var productObjFromDb = _dbContext.Products
                .FirstOrDefault(p => p.ProductId == productObj.ProductId);

            if (productObjFromDb is not null)
            {
                productObjFromDb.Title = productObj.Title;
                productObjFromDb.Description = productObj.Description;
                productObjFromDb.ISBN = productObj.ISBN;
                productObjFromDb.Author = productObj.Author;
                productObjFromDb.ListPrice = productObj.ListPrice;
                productObjFromDb.Price = productObj.Price;
                productObjFromDb.Price50 = productObj.Price50;
                productObjFromDb.Price100 = productObj.Price100;
                productObjFromDb.CategoryId = productObj.CategoryId;
                if (productObj.ImageUrl is not null)
                {
                    productObjFromDb.ImageUrl = productObj.ImageUrl;
                }
            }
        }
    }
}
