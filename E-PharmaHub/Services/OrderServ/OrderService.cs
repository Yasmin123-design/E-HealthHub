using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Services.NotificationServ;
using E_PharmaHub.Services.StripePaymentServ;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace E_PharmaHub.Services.OrderServ
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        public OrderService(IUnitOfWork unitOfWork,IConfiguration config,INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _config = config;

        }
        public async Task<CartResult> CheckoutAsync(string userId, CheckoutDto dto)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId, asNoTracking: true);

            if (cart == null || cart.Items == null || !cart.Items.Any())
                return new CartResult { Success = false, Message = "Cart is empty" };

            var cartItems = await _unitOfWork.CartItemRepository.GetCartItemsWithDetailsByCartIdAsync(cart.Id);

            var itemsForThisPharmacy = new List<dynamic>();

            foreach (var cartItem in cartItems)
            {
                var inventory = await _unitOfWork.IinventoryItem
                    .GetInventoryForCheckoutAsync(cartItem.MedicationId, dto.PharmacyId, cartItem.UnitPrice);

                if (inventory != null)
                {
                    itemsForThisPharmacy.Add(new
                    {
                        CartItem = cartItem,
                        Inventory = inventory
                    });
                }
            }

            if (!itemsForThisPharmacy.Any())
                return new CartResult { Success = false, Message = "No items for this pharmacy" };

            foreach (var x in itemsForThisPharmacy)
            {
                if (x.Inventory.Quantity < x.CartItem.Quantity)
                    return new CartResult
                    {
                        Success = false,
                        Message = $"Not enough quantity for {x.CartItem.Medication.BrandName}"
                    };
            }

            foreach (var x in itemsForThisPharmacy)
            {
                await _unitOfWork.IinventoryItem.DecreaseQuantityAsync(x.Inventory.Id, x.CartItem.Quantity);
            }

            var existingOrder = await _unitOfWork.Order
                .GetPendingOrderEntityByUserForUpdateAsync(userId, dto.PharmacyId);

            bool isNewOrder = existingOrder == null;

            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(dto.PharmacyId);
            decimal deliveryFee = pharmacy?.DeliveryFee ?? 0m;

            decimal itemsTotal = itemsForThisPharmacy.Sum(x => (decimal)(x.CartItem.UnitPrice * x.CartItem.Quantity));
            decimal totalPrice = itemsTotal + deliveryFee;

            if (!isNewOrder)
            {
                foreach (var x in itemsForThisPharmacy)
                {
                    var cartItem = x.CartItem;
                    var existingItem = existingOrder.Items.FirstOrDefault(i => i.MedicationId == cartItem.MedicationId);

                    if (existingItem != null)
                        existingItem.Quantity += cartItem.Quantity;
                    else
                        existingOrder.Items.Add(new OrderItem
                        {
                            MedicationId = cartItem.MedicationId,
                            Quantity = cartItem.Quantity,
                            UnitPrice = cartItem.UnitPrice
                        });
                }

                existingOrder.TotalPrice = totalPrice;
                existingOrder.City = dto.City;
                existingOrder.Country = dto.Country;
                existingOrder.Street = dto.Street;
                existingOrder.PhoneNumber = dto.PhoneNumber;

                await _unitOfWork.Order.UpdateWithItemsAsync(existingOrder);
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
                    TotalPrice = totalPrice,
                    Items = itemsForThisPharmacy.Select(x => new OrderItem
                    {
                        MedicationId = x.CartItem.MedicationId,
                        Quantity = x.CartItem.Quantity,
                        UnitPrice = x.CartItem.UnitPrice
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
                Data = new
                {
                    OrderId = existingOrder.Id,
                    ItemsTotal = itemsTotal,       
                    DeliveryFee = deliveryFee,     
                    TotalPrice = totalPrice       
                }
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

                await _notificationService.CreateAndSendAsync(
               userId: order.UserId,
               title: "Order Confirmed",
               message: $"Your medicine order from pharmacy {order.Pharmacy.Name} has been confirmed",
               type: NotificationType.OrderConfirmed
           );

                return (true, "Order accepted successfully.");


            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> CancelOrderAsync(int id)
        {
            var order = await _unitOfWork.Order.GetByIdForUpdateAsync(id);
            if (order == null)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Confirmed)
                return (false, "Order already accepted.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "Order already cancelled.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(order.PaymentId.Value);

            try
            {
                var paymentIntentService = new PaymentIntentService();
                var intent = await paymentIntentService.GetAsync(payment.PaymentIntentId);

                string notificationMessage = string.Empty;

                if (intent.Status == "canceled")
                {
                    payment.Status = PaymentStatus.Refunded;
                    order.Status = OrderStatus.Cancelled;
                    order.PaymentStatus = PaymentStatus.Refunded;
                    notificationMessage = "Order cancelled successfully (Stripe already canceled the payment).";
                }
                else if (intent.Status == "succeeded" || intent.Status == "requires_capture")
                {
                    var stripeService = new StripePaymentService(_config, _unitOfWork);
                    await stripeService.RefundPaymentAsync(payment.PaymentIntentId);

                    payment.Status = PaymentStatus.Refunded;
                    order.Status = OrderStatus.Cancelled;
                    order.PaymentStatus = PaymentStatus.Refunded;
                    notificationMessage = "Order cancelled and money refunded successfully.";
                }
                else if (intent.Status == "requires_payment_method" || intent.Status == "requires_confirmation" || intent.Status == "requires_action")
                {
                    var stripeService = new StripePaymentService(_config, _unitOfWork);
                    await stripeService.CancelPaymentAsync(payment.PaymentIntentId);

                    payment.Status = PaymentStatus.Failed;
                    order.Status = OrderStatus.Cancelled;
                    order.PaymentStatus = PaymentStatus.Failed;
                    notificationMessage = "Order cancelled before payment was completed.";
                }

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

                await _notificationService.CreateAndSendAsync(
                    userId: order.UserId,
                    title: "Order Cancelled",
                    message: notificationMessage,
                    type: NotificationType.OrderCancelled
                );

                return (true, notificationMessage);
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

            await _notificationService.CreateAndSendAsync(
              userId: order.UserId,
              title: "Order Delivered",
              message: $"Your medicine order from pharmacy {order.Pharmacy.Name} has been delivered successfully",
              type: NotificationType.OrderDelivered
          );

            return (true, "Order delivered successfully.");
        }
    }
}



