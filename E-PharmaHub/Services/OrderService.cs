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

            var existingOrder = await _unitOfWork.Order
                .GetPendingOrderByUserAsync(userId, dto.PharmacyId);

            if (existingOrder != null)
            {
                foreach (var cartItem in cart.Items)
                {
                    var existingItem = existingOrder.Items
                        .FirstOrDefault(i => i.MedicationId == cartItem.MedicationId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += cartItem.Quantity;
                    }
                    else
                    {
                        existingOrder.Items.Add(new OrderItem
                        {
                            MedicationId = cartItem.MedicationId,
                            Quantity = cartItem.Quantity,
                            UnitPrice = cartItem.UnitPrice
                        });
                    }
                }

                existingOrder.TotalPrice = existingOrder.Items.Sum(i => i.UnitPrice * i.Quantity);
            }
            else
            {
                existingOrder = new Order
                {
                    UserId = userId,
                    City = dto.City,
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

                await _unitOfWork.Order.AddAsync(existingOrder);
            }

            await _unitOfWork.Carts.ClearCartAsync(cart);
            await _unitOfWork.CompleteAsync();

            return new CartResult
            {
                Success = true,
                Message = existingOrder.Id > 0
                    ? "Order updated with new items successfully."
                    : "Checkout completed. New order created successfully.",
                Data = new { OrderId = existingOrder.Id, existingOrder.TotalPrice }
            };
        }


        public async Task MarkAsPaid(string userId)
        {
            await _unitOfWork.Order.MarkAsPaid(userId);
            await _unitOfWork.CompleteAsync();
        }
    }
}

