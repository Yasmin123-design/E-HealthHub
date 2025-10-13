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
        public async Task<IEnumerable<BriefOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Order.GetAllAsync();

            return orders.Select(o => new BriefOrderDto
            {
                Id = o.Id,
                Email = o.User.Email,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                Items = o.Items.Select(i => new BriefOrderItemDto
                {
                    MedicationName = i.Medication.BrandName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });
        }

        public async Task<BriefOrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);

            if (order == null)
                return null;

            return new BriefOrderDto
            {
                Id = order.Id,
                Email = order.User.Email,
                PharmacyId = order.PharmacyId,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString(),
                Items = order.Items.Select(i => new BriefOrderItemDto
                {
                    MedicationName = i.Medication.BrandName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

        public async Task<(bool Success, string Message)> AcceptOrderAsync(int id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);
            if (order == null)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Confirmed)
                return (false, "This order is already accepted.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "This order has been cancelled and cannot be accepted.");

            var payment = await _unitOfWork.Payments.GetByReferenceIdAsync(order.Id.ToString());
            if (payment == null)
                return (false, "No payment found for this order.");

            if (payment.Status == PaymentStatus.Succeeded)
                return (false, "Payment already Paid Successfully.");

            try
            {
                var paymentIntentService = new Stripe.PaymentIntentService();
                await paymentIntentService.CaptureAsync(payment.ProviderTransactionId);

                payment.Status = PaymentStatus.Succeeded;
                order.Status = OrderStatus.Confirmed;
                order.PaymentStatus = PaymentStatus.Paid;
                await _unitOfWork.CompleteAsync();

                return (true, "Order accepted and payment captured successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to capture payment: {ex.Message}");
            }
        }


        public async Task<(bool Success, string Message)> CancelOrderAsync(int id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);
            if (order == null)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Confirmed)
                return (false, "This order has already been accepted and cannot be cancelled.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "This order is already cancelled.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(order.PaymentId.Value);
            if (payment == null)
                return (false, "Payment not found for this order.");

            try
            {
                var paymentIntentService = new Stripe.PaymentIntentService();
                await paymentIntentService.CancelAsync(payment.ProviderTransactionId);

                payment.Status = PaymentStatus.Refunded;
                order.Status = OrderStatus.Cancelled;
                order.PaymentStatus = PaymentStatus.Refunded;

                await _unitOfWork.CompleteAsync();

                return (true, "Order cancelled and payment authorization voided successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to cancel payment: {ex.Message}");
            }
        }



        public async Task<IEnumerable<BriefOrderDto>> GetOrdersByPharmacyIdAsync(int pharmacyId)
        {
            var orders = await _unitOfWork.Order.GetByPharmacyId(pharmacyId);

            return orders.Select(o => new BriefOrderDto
            {
                Id = o.Id,
                Email = o.User?.Email ?? "N/A",
                Status = o.Status.ToString(),
                PharmacyId = o.PharmacyId,
                TotalPrice = o.TotalPrice,
                Items = o.Items.Select(i => new BriefOrderItemDto
                {
                    MedicationName = i.Medication.BrandName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();
        }

        public async Task<BriefOrderDto?> GetPendingOrderByUserAsync(string userId, int pharmacyId)
        {
            var order = await _unitOfWork.Order.GetPendingOrderByUserAsync(userId, pharmacyId);
            if (order == null)
                return null;

            return new BriefOrderDto
            {
                Id = order.Id,
                Email = order.User.Email,
                PharmacyId = order.PharmacyId,
                Status = order.Status.ToString(),
                TotalPrice = order.TotalPrice,
                Items = order.Items.Select(i => new BriefOrderItemDto
                {
                    MedicationName = i.Medication.BrandName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }
        public async Task<(bool Success, string Message)> MarkAsDeliveredAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(orderId);
            if (order == null)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Delivered)
                return (false, "Order is already marked as delivered.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "Cannot mark a cancelled order as delivered.");

            if (order.Status != OrderStatus.Confirmed)
                return (false, "Only confirmed orders can be marked as delivered.");

            await _unitOfWork.Order.UpdateStatusAsync(orderId, OrderStatus.Delivered);
            await _unitOfWork.CompleteAsync();

            return (true, "Order marked as delivered successfully.");
        }

        public async Task<IEnumerable<BriefOrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.Order.GetByUserIdAsync(userId);

            if (orders == null || !orders.Any())
                return new List<BriefOrderDto>();

            return orders.Select(o => new BriefOrderDto
            {
                Id = o.Id,
                Email = o.User?.Email ?? "N/A",
                Status = o.Status.ToString(),
                PharmacyId = o.PharmacyId,
                TotalPrice = o.TotalPrice,
                Items = o.Items.Select(i => new BriefOrderItemDto
                {
                    MedicationName = i.Medication.BrandName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();
        }
    }
}


