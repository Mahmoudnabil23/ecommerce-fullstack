import { ProductListItem } from './product.model';
import { OrderListItem } from './order.model';

// ── User Enums ───────────────────────────────────────────────────────────
export enum UserRole {
  Customer = 'Customer',
  Seller = 'Seller',
  Admin = 'Admin',
}

export enum UserStatus {
  Active = 0,
  Suspended = 1,
  Banned = 2,
}

// ── Admin User List Item ─────────────────────────────────────────────────
export interface AdminUserListItem {
  id: string;
  fullName: string;
  email: string;
  roles: string[];
  status: UserStatus;
  createdAt: string;
}

// ── Admin Dashboard ──────────────────────────────────────────────────────
export interface SalesByDay {
  date: string;
  amount: number;
}

export interface AdminDashboard {
  totalRevenue: number;
  totalOrders: number;
  totalUsers: number;
  totalProducts: number;
  recentOrders: OrderListItem[];
  topProducts: ProductListItem[];
  salesByDay: SalesByDay[];
}

// ── Admin Filters ────────────────────────────────────────────────────────
export interface AdminUserFilter {
  search?: string;
  role?: string;
  status?: UserStatus;
  page?: number;
  limit?: number;
}

export interface AdminProductFilter {
  search?: string;
  page?: number;
  limit?: number;
}

export interface UpdateUserStatusRequest {
  status: UserStatus;
}

// ── Paginated Responses (match backend anonymous objects) ────────────────
export interface AdminPaginatedUsers {
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  users: AdminUserListItem[];
}

export interface AdminPaginatedProducts {
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  products: any[];
}
