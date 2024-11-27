﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories;


namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public class ProductService : ValidationAttribute, IProductService
    {
        private readonly ICartService _cart;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IStringLocalizer<ProductService> _localizer;

        public ProductService(ICartService cart, IProductRepository productRepository,
            IOrderRepository orderRepository, IStringLocalizer<ProductService> localizer)
        {
            _cart = cart;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _localizer = localizer;
        }

        public List<ProductViewModel> GetAllProductsViewModel()
        {

            List<Product> productEntities = GetAllProducts();
            return MapToViewModel(productEntities);
        }

        private static List<ProductViewModel> MapToViewModel(List<Product> productEntities)
        {
            List<ProductViewModel> products = new List<ProductViewModel>();
            foreach (Product product in productEntities)
            {
                products.Add(new ProductViewModel
                {
                    Id = product.Id,
                    Stock = product.Quantity.ToString(),
                    Price = product.Price.ToString(CultureInfo.InvariantCulture),
                    Name = product.Name,
                    Description = product.Description,
                    Details = product.Details
                });
            }

            return products;
        }

        public List<Product> GetAllProducts()
        {
            IEnumerable<Product> productEntities = _productRepository.GetAllProducts();
            return productEntities?.ToList();
        }

        public ProductViewModel GetProductByIdViewModel(int id)
        {
            List<ProductViewModel> products = GetAllProductsViewModel().ToList();
            return products.Find(p => p.Id == id);
        }


        public Product GetProductById(int id)
        {

            List<Product> products = GetAllProducts()?.ToList();
            if (products == null)
            {
                return null;
            }
            return products.Find(p => p.Id == id);
        }

        public async Task<Product> GetProduct(int id)
        {
            var product = await _productRepository.GetProduct(id);
            return product;
        }

        public async Task<IList<Product>> GetProduct()
        {
            var products = await _productRepository.GetProduct();
            return products;
        }
        public void UpdateProductQuantities()
        { 
            foreach (CartLine line in _cart.Lines )
            {
                _productRepository.UpdateProductStocks(line.Product.Id, line.Quantity);
            }
        }

        public List<string> CheckProductModelErrors(ProductViewModel product)
        {
            var validationContext = new ValidationContext(product, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            
            bool isValid = Validator.TryValidateObject(product, validationContext, validationResults, validateAllProperties: true);
            
            var errors = new List<string>();
            
            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    if (_localizer != null && !string.IsNullOrEmpty(validationResult.ErrorMessage))
                    {
                        var localizedMessage = _localizer[validationResult.ErrorMessage];
                        errors.Add(localizedMessage ?? validationResult.ErrorMessage);
                    }
                    else
                    {
                        errors.Add(validationResult.ErrorMessage);
                    }
                }
            }
            return errors;
        }

        public void SaveProduct(ProductViewModel product)
        {
            var productToAdd = MapToProductEntity(product);
            _productRepository.SaveProduct(productToAdd);
        }


        public  Product MapToProductEntity(ProductViewModel product)

        {
            Product productEntity = new Product
            {
                Name = product.Name,
                Price = double.Parse(product.Price),
                Quantity = int.Parse(product.Stock),
                Description = product.Description,
                Details = product.Details
            };
            return productEntity;
        }

        public void DeleteProduct(int id)
        {
            var result = _cart.Lines.FirstOrDefault(e => e.Product.Id == id);
            if (result != null)
            {
                _cart.RemoveLine(GetProductById(id));
            }
            _productRepository.DeleteProduct(id);
        }
    }
}
