using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.CartServ
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

            var availableQuantity = inventoryItem.Quantity;

            var existingItem = cart.Items?.FirstOrDefault(i => i.MedicationId == medicationId);

            if (existingItem != null)
            {
                var totalRequested = existingItem.Quantity + quantity;

                if (totalRequested > availableQuantity)
                {
                    return new CartResult
                    {
                        Success = false,
                        Message = $"Only {availableQuantity} units available"
                    };
                }

                existingItem.Quantity = totalRequested;
            }
            else
            {
                if (quantity > availableQuantity)
                {
                    return new CartResult
                    {
                        Success = false,
                        Message = $"Only {availableQuantity} units available"
                    };
                }

                await _unitOfWork.CartItemRepository.AddCartItemAsync(new CartItem
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

            await _unitOfWork.CartItemRepository.RemoveCartItemAsync(item);
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


        public async Task<CartResponseDto> GetUserCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetUserCartAsync(userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                return new CartResponseDto
                {
                    CartId = cart?.Id ?? 0,
                    Pharmacies = new List<CartPharmacyGroupDto>()
                };
            }

            var itemsWithInventory = cart.Items
                .Select(i => new
                {
                    Item = i,
                    Inventory = i.Medication.Inventories
                        .FirstOrDefault(inv => inv.Price == i.UnitPrice)
                })
                .Where(x => x.Inventory != null)
                .ToList();

            var grouped = itemsWithInventory
                .GroupBy(x => x.Inventory.PharmacyId)
                .Select(g =>
                {
                    var pharmacy = g.First().Inventory.Pharmacy;

                    return new CartPharmacyGroupDto
                    {
                        PharmacyId = pharmacy.Id,
                        PharmacyName = pharmacy.Name,
                        DeliveryFee = pharmacy.DeliveryFee ?? 0m,

                        Items = g.Select(x => new CartItemDto
                        {
                            Id = x.Item.Id,
                            Medication = x.Item.Medication.BrandName,
                            MedicationImage = x.Item.Medication.ImagePath,
                            Quantity = x.Item.Quantity,
                            UnitPrice = x.Item.UnitPrice
                        }).ToList()
                    };
                })
                .ToList();

            return new CartResponseDto
            {
                CartId = cart.Id,
                Pharmacies = grouped
            };
        }

        public async Task<(bool, string)> UpdateQuantityAsync(string userId, int cartItemId, int quantity)
        {
            var cartItem = await _unitOfWork.CartItemRepository.GetByIdAsync(cartItemId);

            if (cartItem == null || cartItem.Cart.UserId != userId)
                return (false, "Invalid cart item.");

            cartItem.Quantity = quantity;

            _unitOfWork.CartItemRepository.Update(cartItem);
            await _unitOfWork.CompleteAsync();

            return (true, "Quantity updated.");
        }




    }
}
