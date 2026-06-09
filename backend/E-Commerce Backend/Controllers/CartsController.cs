using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Cart.Requests;
using E_Commerce_Backend.DTOs.Cart.Responses;
using E_Commerce_Backend.Services.Cart;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController(CartService cartService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Result<CartResponseDto>>> GetCart([FromQuery] string? guestSessionId)
        {
            // For simplicity read user id from claims if present
            Guid? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var sub = User.FindFirst("sub")?.Value;
                if (Guid.TryParse(sub, out var id))
                    userId = id;
            }

            // Ensure a cart exists for the caller (creates one if missing) and get its id
            Guid cartId = await cartService.EnsureCartForUserAsync(userId, guestSessionId);

            // If caller is authenticated or provided a guestSessionId, return the full cart
            if (userId.HasValue || !string.IsNullOrEmpty(guestSessionId))
            {
                var dto = await cartService.GetCartAsync(userId, guestSessionId);
                // Make sure CartId is set on the DTO
                if (dto.CartId == Guid.Empty)
                    dto.CartId = cartId;

                return Ok(Result<CartResponseDto>.Ok(dto));
            }

            // Anonymous caller without a guest session: return minimal cart info (cart id)
            var empty = new CartResponseDto { CartId = cartId };
            return Ok(Result<CartResponseDto>.Ok(empty));
        }

        [HttpPost("items")]
        public async Task<ActionResult<Result<object>>> AddItem([FromQuery] Guid? cartId, [FromBody] AddCartItemRequestDto dto)
        {
            if (cartId is null)
                return BadRequest(Result<object>.Fail("cartId query parameter required"));

            var added = await cartService.AddItemAsync(cartId.Value, dto);
            if (added is null)
                return NotFound(Result<object>.Fail("Product not found"));

            return Ok(Result<object>.Ok(null, "Item added"));
        }

        [HttpPut("items/{itemId:guid}")]
        public async Task<ActionResult<Result<object>>> UpdateItem(Guid itemId, [FromQuery] Guid? cartId, [FromBody] UpdateCartItemRequestDto dto)
        {
            if (cartId is null)
                return BadRequest(Result<object>.Fail("cartId query parameter required"));

            var ok = await cartService.UpdateItemAsync(cartId.Value, itemId, dto);
            if (!ok) return NotFound(Result<object>.Fail("Cart item not found"));

            return Ok(Result<object>.Ok(null, "Item updated"));
        }

        [HttpDelete("items/{itemId:guid}")]
        public async Task<ActionResult<Result<object>>> RemoveItem(Guid itemId, [FromQuery] Guid? cartId)
        {
            if (cartId is null)
                return BadRequest(Result<object>.Fail("cartId query parameter required"));

            var ok = await cartService.RemoveItemAsync(cartId.Value, itemId);
            if (!ok) return NotFound(Result<object>.Fail("Cart item not found"));

            return Ok(Result<object>.Ok(null, "Item removed"));
        }

        [HttpPost("merge")]
        public async Task<ActionResult<Result<object>>> Merge([FromQuery] Guid? targetCartId, [FromBody] MergeCartRequestDto dto)
        {
            if (targetCartId is null)
                return BadRequest(Result<object>.Fail("targetCartId query parameter required"));

            var ok = await cartService.MergeCartAsync(targetCartId.Value, dto.GuestSessionId);
            if (!ok) return NotFound(Result<object>.Fail("Merge failed"));

            return Ok(Result<object>.Ok(null, "Carts merged"));
        }
    }
}
