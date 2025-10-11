using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CartResult> CheckoutAsync(string userId, CheckoutDto dto)
        {

            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
                return new CartResult { Success = false, Message = "Cart is empty" };

            var order = new Order
            {

                UserId = userId,
                City=dto.City,
                Country = dto.Country,
                Street = dto.Street,
                PhoneNumber = dto.PhoneNumber,
                PharmacyId = dto.PharmacyId,
                TotalPrice = cart.Items.Sum(i => i.UnitPrice * i.Quantity),
                Items = cart.Items.Select(i => new OrderItem
                {
                    MedicationId = i.MedicationId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _unitOfWork.Order.AddAsync(order);
            await _unitOfWork.Carts.ClearCartAsync(cart);
            await _unitOfWork.CompleteAsync();

            return new CartResult
            {
                Success = true,
                Message = "Checkout completed. Order created successfully.",
                Data = new { OrderId = order.Id, order.TotalPrice }
            };
        }
    }
}

