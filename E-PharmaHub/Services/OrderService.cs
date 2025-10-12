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

        public async Task<bool> AcceptOrderAsync(int id)
        {
            await _unitOfWork.Order.UpdateStatusAsync(id, OrderStatus.Confirmed);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            await _unitOfWork.Order.UpdateStatusAsync(id, OrderStatus.Cancelled);
            await _unitOfWork.CompleteAsync();
            return true;
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


