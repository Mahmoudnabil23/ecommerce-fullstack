// ── Category Ref ─────────────────────────────────────────────────────────
export interface CategoryRef {
  id: string;
  name: string;
}

// ── Seller Ref ───────────────────────────────────────────────────────────
export interface SellerRef {
  id: string;
  storeName: string;
}

// ── Product List Item (matches ProductListItemDto) ───────────────────────
export interface ProductListItem {
  id: string;
  name: string;
  slug: string;
  price: number;
  discountedPrice?: number;
  images: string[];
  category: CategoryRef;
  seller: SellerRef;
  stock: number;
  averageRating: number;
  reviewCount: number;
}

// ── Product Detail (matches ProductDetailResponseDto) ────────────────────
export interface ProductDetail extends ProductListItem {
  description: string;
  specsJson?: string;
  createdAt: string;
}

// ── Product Filters (matches ProductFilterRequestDto) ────────────────────
export interface ProductFilter {
  search?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  sellerId?: string;
  inStock?: boolean;
  sortBy?: 'price_asc' | 'price_desc' | 'newest' | 'rating';
  page?: number;
  limit?: number;
}
