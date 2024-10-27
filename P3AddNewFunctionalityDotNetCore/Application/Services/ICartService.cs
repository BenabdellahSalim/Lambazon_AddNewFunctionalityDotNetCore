using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;

namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public interface ICartService
    {
        void AddItem(Product product, int quantity);

        void RemoveLine(Product product);

        void Clear();

        double GetTotalValue();

        double GetAverageValue();
    }
}