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

            var itemsForThisPharmacy = cart.Items
                .Select(i => new
                {
                    CartItem = i,
                    Inventory = i.Medication.Inventories
                        .FirstOrDefault(inv => inv.Price == i.UnitPrice)
                })
                .Where(x => x.Inventory != null && x.Inventory.PharmacyId == dto.PharmacyId)
                .Select(x => x.CartItem)
                .ToList();

            if (!itemsForThisPharmacy.Any())
                return new CartResult { Success = false, Message = "No items for this pharmacy" };

            var existingOrder = await _unitOfWork.Order
                .GetPendingOrderEntityByUserAsync(userId, dto.PharmacyId);

            bool isNewOrder = existingOrder == null;

            if (!isNewOrder)
            {
                foreach (var cartItem in itemsForThisPharmacy)
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
                existingOrder.City = dto.City;
                existingOrder.Country = dto.Country;
                existingOrder.Street = dto.Street;
                existingOrder.PhoneNumber = dto.PhoneNumber;

                await _unitOfWork.Order.UpdateAsync(existingOrder);
            }
            else
            {
                existingOrder = new Order
                {
                    UserId = userId,
                    PharmacyId = dto.PharmacyId,
                    City = dto.City,
                    Country = dto.Country,
                    Street = dto.Street,
                    PhoneNumber = dto.PhoneNumber,
                    Status = OrderStatus.Pending,
                    TotalPrice = itemsForThisPharmacy.Sum(i => i.UnitPrice * i.Quantity),
                    Items = itemsForThisPharmacy.Select(i => new OrderItem
                    {
                        MedicationId = i.MedicationId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                };

                await _unitOfWork.Order.AddAsync(existingOrder);
            }

            await _unitOfWork.Carts.ClearCartItemsByPharmacyAsync(cart.Id, dto.PharmacyId);

            await _unitOfWork.CompleteAsync();

            return new CartResult
            {
                Success = true,
                Message = isNewOrder
                    ? "Checkout completed. New order created successfully."
                    : "Pending order updated with new items successfully.",
                Data = new { OrderId = existingOrder.Id, existingOrder.TotalPrice }
            };
        }


        public async Task MarkAsPaid(string userId)
        {
            await _unitOfWork.Order.MarkAsPaid(userId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            return await _unitOfWork.Order.GetAllAsync();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            return await _unitOfWork.Order.GetOrderResponseByIdAsync(id);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByPharmacyIdAsync(int pharmacyId)
        {
            return await _unitOfWork.Order.GetByPharmacyIdAsync(pharmacyId);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(string userId)
        {
            return await _unitOfWork.Order.GetByUserIdAsync(userId);
        }


        public async Task<OrderResponseDto?> GetPendingOrderByUserAsync(string userId, int pharmacyId)
        {
            return await _unitOfWork.Order.GetPendingOrderByUserAsync(userId, pharmacyId);
        }

        public async Task<(bool Success, string Message)> AcceptOrderAsync(int id)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(id);
            if (order == null)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Confirmed)
                return (false, "This order is already accepted.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "This order has been cancelled.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(order.PaymentId.Value);
            if (payment == null)
                return (false, "Payment not found.");

            try
            {
                var paymentIntentService = new Stripe.PaymentIntentService();
                await paymentIntentService.CaptureAsync(payment.PaymentIntentId);

                payment.Status = PaymentStatus.Paid;
                order.Status = OrderStatus.Confirmed;
                order.PaymentStatus = PaymentStatus.Paid;

                await _unitOfWork.CompleteAsync();

                return (true, "Order accepted successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> CancelOrderAsync(int id)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(id);
            if (order == null)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Confirmed)
                return (false, "Order already accepted.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "Order already cancelled.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(order.PaymentId.Value);

            try
            {
                var paymentIntentService = new Stripe.PaymentIntentService();
                await paymentIntentService.CancelAsync(payment.PaymentIntentId);

                payment.Status = PaymentStatus.Refunded;
                order.Status = OrderStatus.Cancelled;
                order.PaymentStatus = PaymentStatus.Refunded;

                foreach (var item in order.Items)
                {
                    var invItem = await _unitOfWork.IinventoryItem
                        .GetByPharmacyAndMedicationAsync(order.PharmacyId, item.MedicationId);

                    if (invItem != null)
                    {
                        invItem.Quantity += item.Quantity;
                        await _unitOfWork.IinventoryItem.Update(invItem);
                    }
                }

                await _unitOfWork.CompleteAsync();

                return (true, "Order cancelled successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool Success, string Message)> MarkAsDeliveredAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
            if (order == null)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Delivered)
                return (false, "Already delivered.");

            if (order.Status != OrderStatus.Confirmed)
                return (false, "Only confirmed orders can be delivered.");

            await _unitOfWork.Order.UpdateStatusAsync(orderId, OrderStatus.Delivered);
            await _unitOfWork.CompleteAsync();

            return (true, "Order delivered successfully.");
        }
    }
}



