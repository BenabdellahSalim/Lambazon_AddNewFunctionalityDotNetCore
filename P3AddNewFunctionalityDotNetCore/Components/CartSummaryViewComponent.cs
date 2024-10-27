using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Application.Services;

namespace P3AddNewFunctionalityDotNetCore.Components
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly CartService _cart;

        public CartSummaryViewComponent(ICartService cart)
        {
            _cart = cart as CartService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_cart);
        }
    }
}
