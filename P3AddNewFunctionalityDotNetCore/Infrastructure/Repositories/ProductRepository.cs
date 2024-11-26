using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public  P3Referential Context;

        public ProductRepository(P3Referential context)
        {
            Context = context;
        }
        public async Task<Product> GetProduct(int id)
        {
            var product = await Context.Product.SingleOrDefaultAsync(m => m.Id == id);
            return product;
        }

        public async Task<IList<Product>> GetProduct()
        {
            var products = await Context.Product.ToListAsync();
            return products;
        }
        /// <summary>
        /// Get all products from the inventory
        /// </summary>
        public List<Product> GetAllProducts()
        {
            IEnumerable<Product> productEntities = Context.Product.Where(p => p.Id > 0);
            return productEntities.ToList();
        }

        /// <summary>
        /// Update the stock of a product by its id
        /// </summary>
        public void UpdateProductStocks(int id, int quantityToRemove)
        {
            var product = Context.Product.First(p => p.Id == id);
            product.Quantity = product.Quantity - quantityToRemove;

            if (product.Quantity == 0)
                Context.Product.Remove(product);
            else
            {
                Context.Product.Update(product);
                Context.SaveChanges();
            }
        }

        public void SaveProduct(Product product)
        {
            if (product != null)
            {


                Context.Product.Add(product);
                Context.SaveChanges();
            }
        }



        public void DeleteProduct(int id)
        {
            Product product = Context.Product.First(p => p.Id == id);
            if (product != null)
            {
                Context.Product.Remove(product);
                Context.SaveChanges();
            }
        }
    }
}
