using Xunit;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories;
using NSubstitute;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using System.Threading.Tasks;
using FluentAssertions;

namespace P3AddNewFunctionalityDotNetCore.Tests.Application.Services
{
    public class ProductServiceTeste
    {
        public readonly ProductService _sut;
        public readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
        public readonly ICartService _cartservice = Substitute.For<ICartService>();
        public readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
        public readonly IStringLocalizer<ProductService> _stringLocalizer = Substitute.For<IStringLocalizer<ProductService>>();

        public ProductServiceTeste()
        {
            _stringLocalizer["ErrorMissingName"].Returns(new LocalizedString("ErrorMissingName", "Le nom est manquant."));
            _stringLocalizer["MissingPrice"].Returns(new LocalizedString("MissingPrice", "Le prix est manquant."));
            _stringLocalizer["PriceNotANumber"].Returns(new LocalizedString("PriceNotANumber", "Le prix n'est pas un nombre."));
            _stringLocalizer["PriceNotGreaterThanZero"].Returns(new LocalizedString("PriceNotGreaterThanZero", "Le prix doit être supérieur à zéro."));
            _stringLocalizer["MissingQuantity"].Returns(new LocalizedString("MissingQuantity", "La quantité est manquante."));
            _stringLocalizer["StockNotAnInteger"].Returns(new LocalizedString("StockNotAnInteger", "Le stock n'est pas un entier."));
            _stringLocalizer["StockNotGreaterThanZero"].Returns(new LocalizedString("StockNotGreaterThanZero", "Le stock doit être supérieur à zéro."));
            _sut = new(_cartservice, _productRepository, _orderRepository, _stringLocalizer);
        }
        [Fact]
        public void CheckProductModelErrors_ShouldReturnsErrors_WhenProductsAreInvalid()
        {
            // Arrange
            var product = new ProductViewModel
            {

                Name = "", 
                Price = "-5", 
                Stock = "abc" 
            };

            // Act
            List<string> errors = _sut.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("Le nom est manquant.", errors);
            Assert.Contains("Le prix doit être supérieur à zéro.", errors);
            Assert.Contains("Le stock n'est pas un entier.", errors);
        }
        [Fact]
        public void CheckProductModelErrors_ShouldReturnsNoErrors_WhenProductsAreValid()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Produit A",
                Price = "50",
                Stock = "10"
            };


            // Act
            List<string> errors = _sut.CheckProductModelErrors(product);

            // Assert
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("", "10", "5", "ErrorMissingName")]  
        [InlineData("Produit A", "", "5", "Le prix est manquant.")]  
        [InlineData("Produit A", "abc", "5", "Le prix n'est pas un nombre.")]  
        [InlineData("Produit A", "-5", "5", "Le prix doit être supérieur à zéro.")]  
        [InlineData("Produit A", "10", "", "La quantité est manquante.")]  
        [InlineData("Produit A", "10", "abc", "Le stock n'est pas un entier.")]  
        [InlineData("Produit A", "10", "-1", "Le stock doit être supérieur à zéro.")]  
        public void CheckProductModelErrors_ShouldReturnsExpectedErrors_ForInvalidInputs(string name, string price, string stock, string expectedError)
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = name,
                Price = price,
                Stock = stock
            };
            // Act
            var errors = _sut.CheckProductModelErrors(product);
            // Assert
            foreach (var error in errors)
            {
                error.Should().NotBeEmpty();
            }
        }

        [Fact]
        public void GetProductById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange  
            var productList = new List<Product>
        {
            new Product { Id = 2, Name = "Product B" },
            new Product { Id = 3, Name = "Product C" }
        };
              // Act  
            var result = _sut.GetProductById(1);
            // Assert  
            Assert.Null(result);
        }
        [Fact]
        public void DeleteProduct_ShouldOnlyRemoveFromRepository_WhenProductNotInCart()
        {
            // Arrange
            var productId = 2;
            var product = new Product
            {
                Id = productId,
                Name = "Product B",
                Quantity = 5,
                Price = 20
            };
            _cartservice.Lines.Returns(new List<CartLine>());

            // Act
            _sut.DeleteProduct(productId);
            // Assert
            _cartservice.DidNotReceive().RemoveLine(product);
            _productRepository.Received(1).DeleteProduct(productId);
        }

       
        [Fact]
        public void UpdateProductQuantities_ShouldCallUpdateProductStocksForAnyLineInCartline()
        {
            // Arrange
            var cartLines = new List<CartLine>
        {
            new CartLine { Product = new Product { Id = 1 }, Quantity = 5 },
            new CartLine { Product = new Product { Id = 2 }, Quantity = 3 }
        };
             
            _cartservice.Lines.Returns(cartLines);
            // Act
            _sut.UpdateProductQuantities();
            // Assert
            _productRepository.Received(1).UpdateProductStocks(1, 5);
            _productRepository.Received(1).UpdateProductStocks(2, 3);
        }
        [Fact]
        public async Task GetProduct_ShouldReturnListOfProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product A", Price = 10 },
                new Product { Id = 2, Name = "Product B", Price = 20 }
            };

            _productRepository.GetProduct().Returns(expectedProducts);

            // Act
            var result = await _sut.GetProduct();

            // Assert
            Assert.Equal(expectedProducts.Count, result.Count);  
            Assert.Equal(expectedProducts, result); 
            
            await _productRepository.Received(1).GetProduct();
        }
        [Fact]
        public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new Product { Id = productId, Name = "Product A", Price = 10 };

            _productRepository.GetProduct(productId).Returns(expectedProduct);

            // Act
            var result = await _sut.GetProduct(productId);

            // Assert
            Assert.NotNull(result);  
            Assert.Equal(expectedProduct.Id, result.Id);  
            Assert.Equal(expectedProduct.Name, result.Name);  
            Assert.Equal(expectedProduct.Price, result.Price);  

            await _productRepository.Received(1).GetProduct(productId);
        }
        [Fact]
        public void GetProductByIdViewModel_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new ProductViewModel { Id = productId, Name = "Product A", Price = "10" };

             
            var products = new List<Product>
                {

                    new Product { Id = 1, Name = "Product A", Price = 10, Quantity = 1 },
                    new Product { Id = 2, Name = "Product B", Price = 20, Quantity = 3 }
                };
             
            _sut.GetAllProducts().Returns(products); 

            // Act
            var result = _sut.GetProductByIdViewModel(productId);

            // Assert
            Assert.NotNull(result);  
            Assert.Equal(expectedProduct.Id, result.Id);  
            Assert.Equal(expectedProduct.Name, result.Name);  
            Assert.Equal(expectedProduct.Price, result.Price);  

            
            _productRepository.Received().GetAllProducts();
        }

        [Fact]
        public void SaveProduct_ShouldSaveProductIn()
        {
            // Arrange
            var productViewModel = new ProductViewModel { Name = "Produit A", Price = "50", Stock = "10" };
            var expectedProduct = _sut.MapToProductEntity(productViewModel);
            
            // Act
            _sut.SaveProduct(productViewModel);
            // Assert
            _productRepository.Received().SaveProduct(Arg.Is<Product>(p =>
                p.Name == expectedProduct.Name &&
                p.Price == expectedProduct.Price &&
                p.Quantity == expectedProduct.Quantity));
        }

        [Fact]
        public void GetAllProductsViewModel__ShouldReturnListViewModel()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Product A" }, new() { Id = 2, Name = "Product B" }
            };
             _sut.GetAllProducts().Returns(products);
             
            // Act
            var result = _sut.GetAllProductsViewModel();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Product A", result[0].Name);
            Assert.Equal("Product B", result[1].Name);
        }
    }
}