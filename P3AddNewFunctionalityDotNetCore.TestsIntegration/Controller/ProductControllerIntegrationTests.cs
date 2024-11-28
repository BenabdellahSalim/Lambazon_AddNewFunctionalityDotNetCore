using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NSubstitute;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories;

namespace P3AddNewFunctionalityDotNetCore.TestsIntegration.Controller
{
    public class ProductControllerIntegrationTests : DataBaseInMemory
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
        //    context.Database.EnsureCreated(); 
        //    return context;
        //}

        public ProductControllerIntegrationTests()
        {
            dbContext = GetInMemoryDbContext(); 
            _cart = new CartService();
            productRepository = new ProductRepository(dbContext); 
            order = new OrderRepository(dbContext); 
            productService = new ProductService(_cart, productRepository, order, _stringLocalizer);
            languageService = new LanguageService(); 
        }

        [Fact]
        public void Create_ValidProduct_ShouldAddProductToDatabase()
        {
            //Arrange
//            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureDeletedAsync();
            dbContext.Database.EnsureCreatedAsync();
            var controller = new ProductController(productService, languageService);
            var newProduct = new ProductViewModel
            {
                Name = "Test Product",
                Description = "Test Description",
                Details = "Test Details",
                Stock = "10", 
                Price = "15" 
            };

            // Act 
            var result = controller.Create(newProduct) as RedirectToActionResult;

            // Assert 
            Assert.NotNull(result); 
            Assert.Equal("Admin", result.ActionName); 

            var addedProduct = dbContext.Product.FirstOrDefault(p => p.Name == "Test Product");
            Assert.NotNull(addedProduct); 
            Assert.Equal("Test Product", addedProduct.Name);

            dbContext.Database.EnsureDeletedAsync();
            dbContext.SaveChangesAsync();
        }
        [Fact]
        public void Create_InvalidProduct_ShouldReturnModelErrors()
        {
            // Arrange 
            var controller = new ProductController(productService, languageService);
            var invalidProduct = new ProductViewModel
            {
                Name = "", 
                Stock = "absc",
                Price = "-5" 
            };

            // Act 
            var result = controller.Create(invalidProduct) as ViewResult;

            // Assert 
            Assert.NotNull(result); 
            Assert.False(controller.ModelState.IsValid); 

            var errors = controller.ModelState.Values.SelectMany(v => v.Errors).ToList();
            Assert.Contains(errors, e => e.ErrorMessage.Contains("ErrorMissingName"));
            Assert.Contains(errors, e => e.ErrorMessage.Contains("StockNotAnInteger"));
            Assert.Contains(errors, e => e.ErrorMessage.Contains("PriceNotGreaterThanZero"));

            dbContext.Database.EnsureDeletedAsync();
            dbContext.SaveChangesAsync();
        }

        [Fact]
        public void DeleteProduct_ShouldRedirectToAdmin2()
        {
            // Arrange
            var controller = new ProductController(productService, languageService);
            var product = new ProductViewModel
            {
                Id = 1,
                Name = "Test2 product",
                Stock = "1522",
                Price = "52"
            };
            // Act
            var result = controller.Create(product) as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Admin", result.ActionName);

            var addedProduct = dbContext.Product.FirstOrDefault(p => p.Id == 1);
            Assert.NotNull(addedProduct); 
            Assert.Equal("Test2 product", addedProduct.Name);

            var delProduct = controller.DeleteProduct(1) as RedirectToActionResult;
            Assert.NotNull(delProduct);
            Assert.Equal("Admin", delProduct.ActionName);

            var deletedProduct = dbContext.Product.FirstOrDefault(p => p.Id == 1);
            Assert.Null(deletedProduct); 

            var productControllerSubstitute = Substitute.For<ProductControllerIntegrationTests>();
            productControllerSubstitute.Received(2).GetInMemoryDbContext();
        }

    }
}
