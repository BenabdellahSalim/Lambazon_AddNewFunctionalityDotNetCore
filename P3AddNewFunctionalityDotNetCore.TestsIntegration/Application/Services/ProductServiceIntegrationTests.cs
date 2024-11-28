using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories;

namespace P3AddNewFunctionalityDotNetCore.TestsIntegration.Application.Services
{
    public class ProductServiceIntegrationTests : DataBaseInMemory
    {

        public readonly IStringLocalizer<ProductService> _stringLocalizer;
        private P3Referential dbContext;
        private CartService _cart;
        private ProductRepository productRepository;
        private OrderRepository order;
        private ProductService productService;
        private LanguageService languageService;


        //public P3Referential GetInMemoryDbContext()
        //{
        //    var options = new DbContextOptionsBuilder<P3Referential>()
        //        .UseInMemoryDatabase(databaseName: "TestDatabase")
        //        .Options;

        //    var context = new P3Referential(options);
        //    context.Database
        //        .EnsureCreated();
        //    return context;
        //}


        public ProductServiceIntegrationTests()
        {
            dbContext = GetInMemoryDbContext();
            _cart = new CartService();
            productRepository = new ProductRepository(dbContext);
            order = new OrderRepository(dbContext);
            productService = new ProductService(_cart, productRepository, order, _stringLocalizer);
            languageService = new LanguageService();

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
            dbContext.Database.EnsureDeletedAsync();
            dbContext.SaveChangesAsync();

        }

        [Fact]
        public void Delete_ShouldDeleteProductFromStock()
        {
            // Arrange
          


            var product = new Product { Id = 1, Name = "Test Product", Price = 10, Quantity = 2 };
            dbContext.Product.Add(product);
            dbContext.SaveChangesAsync();
            
            // Act
            productService.DeleteProduct(1);

            // Assert
            var deletedProduct = dbContext.Product.FirstOrDefault(p => p.Id == 1);
            Assert.Null(deletedProduct);

            dbContext.Database.EnsureDeletedAsync();
            dbContext.SaveChangesAsync();
        }

    }
}

