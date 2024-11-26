using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public class CartService : ICartService
    {
        public readonly List<CartLine> _cartLines;
       public virtual IEnumerable<CartLine> Lines => _cartLines;

        public CartService()
        {
            _cartLines = new List<CartLine>();
        }

        public void AddItem(Product product, int quantity)
        {
            CartLine line = _cartLines.FirstOrDefault(p => p.Product.Id == product.Id);

            if (line == null)
            {
                _cartLines.Add(new CartLine { Product = product, Quantity = quantity });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product) => _cartLines.RemoveAll(l => l.Product.Id == product.Id);

        public double GetTotalValue()
        {
            return _cartLines.Any() ? _cartLines.Sum(l => l.Product.Price) : 0;
        }

        public double GetAverageValue()
        {
            return _cartLines.Any() ? _cartLines.Average(l => l.Product.Price) : 0;
        }

        public void Clear() => _cartLines.Clear();

    }
}
