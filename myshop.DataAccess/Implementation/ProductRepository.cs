using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.Implementation
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productInDb = _context.Products.FirstOrDefault(x=>x.Id== product.Id);
            if (productInDb != null)
            {
                productInDb.Name = product.Name;
                productInDb.Description = product.Description;
                productInDb.Price = product.Price;
                if (!string.IsNullOrEmpty(product.Img))
                {
                    productInDb.Img = product.Img;
                }
                productInDb.CategoryId = product.CategoryId;
            }
        }
    }
}
