using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartResult> AddToCartAsync(string userId, int medicationId, int pharmacyId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.CompleteAsync();
            }

            var inventoryItem = await _unitOfWork.IinventoryItem
                .GetByPharmacyAndMedicationAsync(pharmacyId, medicationId);

            if (inventoryItem == null)
                return new CartResult { Success = false, Message = "Medication not available in selected pharmacy" };

            var existingItem = cart.Items?.FirstOrDefault(i => i.MedicationId == medicationId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                await _unitOfWork.Carts.AddCartItemAsync(new CartItem
                {
                    CartId = cart.Id,
                    MedicationId = medicationId,
                    Quantity = quantity,
                    UnitPrice = inventoryItem.Price
                });
            }

            await _unitOfWork.CompleteAsync();
            return new CartResult { Success = true, Message = "Item added to cart successfully" };
        }


        public async Task<CartResult> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null)
                return new CartResult { Success = false, Message = "Cart not found" };

            var item = cart.Items?.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                return new CartResult { Success = false, Message = "Item not found" };

            await _unitOfWork.Carts.RemoveCartItemAsync(item);
            await _unitOfWork.CompleteAsync();

            return new CartResult { Success = true, Message = "Item removed from cart" };
        }

        public async Task<CartResult> ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null)
                return new CartResult { Success = false, Message = "Cart not found" };

            await _unitOfWork.Carts.ClearCartAsync(cart);
            await _unitOfWork.CompleteAsync();

            return new CartResult { Success = true, Message = "Cart cleared successfully" };
        }

        public async Task<object> GetUserCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);
            if (cart == null)
                return new { Message = "Cart is empty" };

            return new
            {
                cart.Id,
                Items = cart.Items.Select(i => new
                {
                    i.Id,
                    Medication = i.Medication?.BrandName,
                    i.Quantity,
                    i.UnitPrice,
                    Total = i.Quantity * i.UnitPrice
                }),
                TotalPrice = cart.Items.Sum(i => i.UnitPrice * i.Quantity)
            };
        }

    }
}
