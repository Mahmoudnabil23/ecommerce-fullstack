// ── Cart Product Ref ─────────────────────────────────────────────────────
export interface CartProductRef {
  id: string;
  name: string;
  price: number;
  imageUrl?: string;
}

// ── Cart Item ────────────────────────────────────────────────────────────
export interface CartItem {
  cartItemId: string;
  product: CartProductRef;
  quantity: number;
  subtotal: number;
}

// ── Cart Summary ─────────────────────────────────────────────────────────
export interface CartSummary {
  subtotal: number;
  discount: number;
  shipping: number;
  total: number;
}

// ── Cart Response ────────────────────────────────────────────────────────
export interface CartResponse {
  cartId: string;
  items: CartItem[];
  summary: CartSummary;
  appliedPromoCode?: string;
}

// ── Cart Requests ────────────────────────────────────────────────────────
export interface AddCartItemRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}

