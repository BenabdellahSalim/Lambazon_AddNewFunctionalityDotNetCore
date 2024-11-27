using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NSubstitute;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories;

namespace ProductServiceIntegrationTests
{
    public class ProductServiceIntegrationTests
    {
        private readonly P3Referential dbContext;
        private readonly ProductRepository productRepository;
        private readonly OrderRepository orderRepository;
        private readonly CartService cartService;
        private readonly ProductService productService;
        private readonly IStringLocalizer<ProductService> stringLocalizer;

        public ProductServiceIntegrationTests()
        {
            dbContext = GetInMemoryDbContext(); // Initialisez le contexte ici
            cartService = new CartService();
            productRepository = new ProductRepository(dbContext);
            orderRepository = new OrderRepository(dbContext);
            stringLocalizer = Substitute.For<IStringLocalizer<ProductService>>();

            productService = new ProductService(cartService, productRepository, orderRepository, stringLocalizer);
        }

        private P3Referential GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<P3Referential>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            var context = new P3Referential(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void SaveProduct_ShouldSaveProductSuccessfully()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "New Product",
                Price = "5",
                Stock = "3"
            };

            // Act
            productService.SaveProduct(product);

            // Assert
            var savedProduct = dbContext.Product.FirstOrDefault(p => p.Name == "New Product");
            Assert.NotNull(savedProduct);
            Assert.Equal("New Product", savedProduct.Name);
            Assert.Equal(5, savedProduct.Price);
        }

        [Fact]
        public void Delete_ShouldDeleteProductFromStock()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product", Price = 10, Quantity = 2 };
            dbContext.Product.Add(product);
            dbContext.SaveChanges();

            // Act
            productService.DeleteProduct(1);

            // Assert
            var deletedProduct = dbContext.Product.FirstOrDefault(p => p.Id == 1);
            Assert.Null(deletedProduct);
        }
    }
}
