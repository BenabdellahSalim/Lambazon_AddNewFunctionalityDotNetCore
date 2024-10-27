using System.Collections.Generic;
using System.Threading.Tasks;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public interface IOrderService
    {
        void SaveOrder(OrderViewModel order);
        Task<Order> GetOrder(int id);
        Task<IList<Order>> GetOrders();
    }
}
