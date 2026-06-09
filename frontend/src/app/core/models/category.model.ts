// ── Category Item (matches CategoryResponseDto) ─────────────────────────
export interface CategoryItem {
  id: string;
  name: string;
  slug: string;
  imageUrl?: string;
  children: CategoryItem[];
}
