using System.Collections.Generic;
using System.Threading.Tasks;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        List<ProductViewModel> GetAllProductsViewModel();
        Product GetProductById(int id);
        ProductViewModel GetProductByIdViewModel(int id);
        void UpdateProductQuantities();
        void SaveProduct(ProductViewModel product);
        void DeleteProduct(int id);
        List<string> CheckProductModelErrors(ProductViewModel product);
        Task<Product> GetProduct(int id);
        Task<IList<Product>> GetProduct();
    }
}
