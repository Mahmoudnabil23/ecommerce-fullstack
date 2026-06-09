import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';
import { sellerGuard } from './core/guards/seller.guard';
import { nonAdminGuard } from './core/guards/non-admin.guard';

// Layouts
import { MainLayoutComponent } from './shared/layouts/main-layout/main-layout.component';
import { AdminLayoutComponent } from './shared/layouts/admin-layout/admin-layout.component';

// Auth
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
import { VerifyEmailComponent } from './features/auth/verify-email/verify-email.component';

// Features
import { HomeComponent } from './features/home/home.component';
import { RoleHomeRedirectComponent } from './features/home/role-home-redirect.component';
import { CustomerHomeComponent } from './features/home/customer-home.component';
import { SellerHomeComponent } from './features/home/seller-home.component';
import { ProductListComponent } from './features/products/product-list/product-list.component';
import { ProductDetailComponent } from './features/products/product-detail/product-detail.component';
import { CartComponent } from './features/cart/cart.component';
import { CheckoutComponent } from './features/checkout/checkout.component';
import { ProfileComponent } from './features/profile/profile.component';
import { OrderHistoryComponent } from './features/profile/order-history/order-history.component';
import { OrderDetailComponent } from './features/profile/order-detail/order-detail.component';

// Admin
import { AdminDashboardComponent } from './features/admin/dashboard/admin-dashboard.component';
import { AdminProductsComponent } from './features/admin/product-management/admin-products.component';
import { AdminUsersComponent } from './features/admin/user-management/admin-users.component';
import { AdminCategoriesComponent } from './features/admin/category-management/admin-categories.component';

export const routes: Routes = [
  // Auth pages (no navbar/footer)
  { path: 'login', component: LoginComponent, title: 'Sign In — AssiutCart' },
  { path: 'register', component: RegisterComponent, title: 'Create Account — AssiutCart' },
  { path: 'forgot-password', component: ForgotPasswordComponent, title: 'Forgot Password — AssiutCart' },
  { path: 'reset-password', component: ResetPasswordComponent, title: 'Reset Password — AssiutCart' },
  { path: 'verify-email', component: VerifyEmailComponent, title: 'Verify Email — AssiutCart' },

  // Admin panel
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [authGuard, adminGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardComponent, title: 'Dashboard — Admin' },
      { path: 'products', component: AdminProductsComponent, title: 'Products — Admin' },
      { path: 'users', component: AdminUsersComponent, title: 'Users — Admin' },
      { path: 'categories', component: AdminCategoriesComponent, title: 'Categories — Admin' },
    ],
  },

  // Customer-facing pages (with navbar/footer)
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [nonAdminGuard],
    children: [
      { path: '', component: RoleHomeRedirectComponent },
      { path: 'welcome', component: HomeComponent, title: 'AssiutCart — Premium E-Commerce' },
      { path: 'customer/home', component: CustomerHomeComponent, canActivate: [authGuard], title: 'My Home — AssiutCart' },
      { path: 'seller/dashboard', component: SellerHomeComponent, canActivate: [authGuard, sellerGuard], title: 'Seller Dashboard — AssiutCart' },
      { path: 'products', component: ProductListComponent, title: 'Products — AssiutCart' },
      { path: 'products/:id', component: ProductDetailComponent, title: 'Product Details — AssiutCart' },
      { path: 'cart', component: CartComponent, title: 'Shopping Cart — AssiutCart' },
      { path: 'checkout', component: CheckoutComponent, canActivate: [authGuard], title: 'Checkout — AssiutCart' },
      { path: 'profile', component: ProfileComponent, canActivate: [authGuard], title: 'My Account — AssiutCart' },
      { path: 'profile/orders', component: OrderHistoryComponent, canActivate: [authGuard], title: 'My Orders — AssiutCart' },
      { path: 'profile/orders/:id', component: OrderDetailComponent, canActivate: [authGuard], title: 'Order Details — AssiutCart' },
    ],
  },

  // Wildcard redirect
  { path: '**', redirectTo: '' },
];
