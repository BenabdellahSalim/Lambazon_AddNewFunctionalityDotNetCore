using Xunit;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Controllers;

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly IProductService _productService;
    private readonly ILanguageService _languageService;

    public ProductControllerTests()
    {
        // Arrange
        _productService = Substitute.For<IProductService>();
        _languageService = Substitute.For<ILanguageService>();
        _controller = new ProductController(_productService, _languageService);
    }


    [Fact]
    public void DeleteProduct_ShouldRedirectToAdmin()
    {
        // Arrange
        int productId = 1;

        // Act
        var result = _controller.DeleteProduct(productId) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Admin", result.ActionName);
        _productService.Received(1).DeleteProduct(productId);  
    }
    [Fact]
    public void Create_ShouldReturnViewWithModel_WhenModelµIsNotValid()
    {
        // Arrange
        var product = new ProductViewModel
        {
            Name = "Test Product",
            Description = "Test Description",
            Details = "Test Details",
            Stock = "invalid_stock", 
            Price = "-10"             
        };

        var errors = new List<string> { "The value entered for the price must be a number", "Please enter a price value" };
       _productService.CheckProductModelErrors(product).Returns(errors);

        // Act
        var result = _controller.Create(product) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product, result.Model);  
        Assert.False(_controller.ModelState.IsValid); 
        Assert.Equal(errors.Count, _controller.ModelState.ErrorCount);  
        foreach (var error in errors)
        {
            Assert.Contains(error, _controller.ModelState[string.Empty].Errors[errors.IndexOf(error)].ErrorMessage);
        }
    }

    [Fact]
    public void Create_ShouldRedirectToAdmin_WhenModelIsValid()
    {
        // Arrange
        var product = new ProductViewModel
        {
            Name = "Valid Product",
            Description = "Valid Description",
            Details = "Valid Details",
            Stock = "10",
            Price = "20.5"
        };
        _productService.CheckProductModelErrors(product).Returns(new List<string>());

        // Act
        var result = _controller.Create(product) as RedirectToActionResult;

        // Assert
        Assert.True(_controller.ModelState.IsValid);
        Assert.NotNull(result);
        Assert.Equal("Admin", result.ActionName);  
        _productService.Received(1).SaveProduct(product);  
    }
}
