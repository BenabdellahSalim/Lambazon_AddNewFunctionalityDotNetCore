using System.Collections.Generic;
using System.Threading.Tasks;
using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;

namespace P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        void Save(Order order);
        Task<Order> GetOrder(int? id);
        Task<IList<Order>> GetOrders();
    }
}
