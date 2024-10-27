using P3AddNewFunctionalityDotNetCore.Data.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Order = P3AddNewFunctionalityDotNetCore.Data.Models.Entities.Order;

namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICartService _cart;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;

        public OrderService(ICartService cart, IOrderRepository orderRepository, IProductService productService)
        {
            _orderRepository = orderRepository;
            _cart = cart;
            _productService = productService;
        }
        public async Task<Order> GetOrder(int id)
        {
            var orderEntity = await _orderRepository.GetOrder(id);
            return orderEntity;
        }
        public async Task<IList<Order>> GetOrders()
        {
            var orders = await _orderRepository.GetOrders();
            return orders;
        }
        public void SaveOrder(OrderViewModel order)
        {
            var orderToAdd = MapToOrderEntity(order);
            _orderRepository.Save(orderToAdd);
            UpdateInventory();
        }

        private static Order MapToOrderEntity(OrderViewModel order)
        {
            Order orderToAdd = new Order
            {
                Name = order.Name,
                Address = order.Address,
                City = order.City,
                Zip = order.Zip,
                Country = order.Country,
                Date = DateTime.UtcNow,
                OrderLine = new List<OrderLine>()
            };
            foreach (var orderLine in order.Lines)
            {
                OrderLine lineOrder = new OrderLine { ProductId = orderLine.Product.Id, Quantity = orderLine.Quantity };
                orderToAdd.OrderLine.Add(lineOrder);
            }

            return orderToAdd;
        }

        private void UpdateInventory()
        {
            _productService.UpdateProductQuantities();
            _cart.Clear();
        }
    }
}
