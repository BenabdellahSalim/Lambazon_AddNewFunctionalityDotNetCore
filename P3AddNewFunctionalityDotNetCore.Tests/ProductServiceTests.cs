using Xunit;
using Moq;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories;
using NSubstitute;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace P3AddNewFunctionalityDotNetCore.Tests
{


    public class ProductServiceTeste
    {
        public readonly ProductService _sut;
        public readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
        public readonly ICartService _cartService = Substitute.For<ICartService>();
        public readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
        public readonly IStringLocalizer<ProductService> _stringLocalizer = Substitute.For<IStringLocalizer<ProductService>>();

        public ProductServiceTeste()
        {
            _sut = new(_cartService, _productRepository, _orderRepository, _stringLocalizer);
        }

        [Theory]
        [InlineData(null, "10", "5", "MissingName")]
        [InlineData("Product A", null, "5", "MissingPrice")]
        [InlineData("Product A", "10", null, "MissingQuantity")]
        [InlineData("Product A", "a", "5", "PriceNotANumber")]
        [InlineData("Product A", "0", "5", "PriceNotGreaterThanZero")]
        [InlineData("Product A", "10", "0", "StockNotGreaterThanZero")]
        [InlineData("Product A", "10", "b", "StockNotAnInteger")]
        public void CheckProductModelErrors_ShouldReturnExpectedError(string name, string price, string stock, string expectedError)
        {
            // Arrange
            var product = new ProductViewModel { Name = name, Price = price, Stock = stock };
            _stringLocalizer[expectedError].Returns(new LocalizedString(expectedError, expectedError));

            // Act
            var errors = _sut.CheckProductModelErrors(product);

            // Assert
            Assert.Contains(expectedError, errors);
        }

        [Fact]
        public void CheckProductModelErrors_ShouldReturnEmptyList_WhenValidProduct()
        {
            // Arrange
            var product = new ProductViewModel { Name = "Valid Product", Price = "100", Stock = "10" };

            // Act
            var errors = _sut.CheckProductModelErrors(product);

            // Assert
            Assert.Empty(errors);
        }
    }
}