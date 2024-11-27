using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using System.Collections.Generic;

namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public interface ICartService
    {
        public IEnumerable<CartLine>  Lines { get; }


        IEnumerable<CartLine> Lines { get; }
        void AddItem(Product product, int quantity);

        void RemoveLine(Product product);

        void Clear();

        double GetTotalValue();

        double GetAverageValue();
    }
}