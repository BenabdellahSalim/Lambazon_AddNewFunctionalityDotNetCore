using System.Linq;
using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;

namespace P3AddNewFunctionalityDotNetCore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cart;
        private readonly IProductService _productService;

        public CartController(ICartService cart, IProductService productService)
        {
            _cart = cart;
            _productService = productService;
        }

        public ViewResult Index()
        {
            CartService cart = _cart as CartService;
            return View(cart);
        }

        [HttpPost]
        public RedirectToActionResult AddToCart(int id)
        {
            Product product = _productService.GetProductById(id);

            if (product != null)
            {
                _cart.AddItem(product, 1);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }

        public RedirectToActionResult RemoveFromCart(int id)
        {
            Product product = _productService.GetAllProducts()
                .FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                _cart.RemoveLine(product);
            }

            return RedirectToAction("Index");
        }
    }
}
