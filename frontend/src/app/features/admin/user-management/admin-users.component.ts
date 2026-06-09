import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../core/services/admin.service';
import { ToastService } from '../../../core/services/toast.service';
import { AdminUserListItem } from '../../../core/models/user.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  template: `
    <div>
      <div class="page-top">
        <h1 class="heading-3">Users</h1>
        <div class="top-controls">
          <select class="input filter-select" [(ngModel)]="roleFilter" (ngModelChange)="onFilterChange()" id="admin-filter-role">
            <option value="">All Roles</option>
            <option value="Customer">Customer</option>
            <option value="Seller">Seller</option>
            <option value="Admin">Admin</option>
          </select>
          <div class="search-bar">
            <input type="text" class="input" placeholder="Search users..." [(ngModel)]="search" (input)="onSearch()" id="admin-search-users"/>
          </div>
        </div>
      </div>

      @if (isLoading) {
        <app-loading-spinner />
      } @else if (errorMessage) {
        <div class="empty-state card">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#ef4444" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>
          <p class="body-small text-muted" style="margin-top: var(--space-4);">{{ errorMessage }}</p>
          <button class="btn btn-ghost btn-sm" style="margin-top: var(--space-4);" (click)="loadUsers()">Retry</button>
        </div>
      } @else if (users.length === 0) {
        <div class="empty-state card">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color: var(--color-shade-50);"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/></svg>
          <p class="body-small text-muted" style="margin-top: var(--space-4);">No users found</p>
        </div>
      } @else {
        <div class="table-card card">
          <table class="admin-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Roles</th>
                <th>Status</th>
                <th>Joined</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (user of users; track user.id) {
                <tr>
                  <td class="body-small">{{ user.fullName }}</td>
                  <td class="caption text-muted">{{ user.email }}</td>
                  <td class="caption">
                    @for (role of user.roles; track role) {
                      <span class="role-badge">{{ role }}</span>
                    }
                  </td>
                  <td>
                    <select class="status-select input" [ngModel]="user.status" (ngModelChange)="updateStatus(user.id, $event)" [id]="'status-' + user.id">
                      <option [ngValue]="0">Active</option>
                      <option [ngValue]="1">Suspended</option>
                      <option [ngValue]="2">Banned</option>
                    </select>
                  </td>
                  <td class="caption text-muted">{{ user.createdAt | date:'mediumDate' }}</td>
                  <td>
                    <button class="btn btn-danger btn-sm" (click)="deleteUser(user.id)" [id]="'delete-user-' + user.id">Delete</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>

        <div class="pagination" style="margin-top: var(--space-7);">
          <button class="btn btn-ghost btn-sm" [disabled]="page <= 1" (click)="goToPage(page - 1)">← Previous</button>
          <span class="text-muted caption">Page {{ page }} of {{ totalPages }} · {{ totalItems }} users</span>
          <button class="btn btn-ghost btn-sm" [disabled]="page >= totalPages" (click)="goToPage(page + 1)">Next →</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .page-top { display: flex; justify-content: space-between; align-items: center; gap: var(--space-4); margin-bottom: var(--space-7); flex-wrap: wrap; }
    .top-controls { display: flex; gap: var(--space-3); align-items: center; flex-wrap: wrap; }
    .search-bar { width: 280px; }
    .filter-select { width: 150px; }
    .table-card { overflow-x: auto; }
    .admin-table { width: 100%; border-collapse: collapse; }
    .admin-table th { text-align: left; padding: var(--space-4) var(--space-5); font-size: 12px; font-weight: 500; color: var(--color-muted); text-transform: uppercase; letter-spacing: 0.72px; border-bottom: 1px solid var(--color-shade-70); }
    .admin-table td { padding: var(--space-4) var(--space-5); border-bottom: 1px solid var(--color-dark-card-border); vertical-align: middle; }
    .admin-table tr:hover td { background: rgba(255,255,255,0.02); }
    .role-badge { font-size: 11px; font-weight: 600; padding: 3px 8px; border-radius: var(--radius-pill); background: rgba(54, 244, 164, 0.1); color: var(--color-neon-green); margin-right: 4px; }
    .status-select { padding: 6px 12px; font-size: 13px; width: 130px; }
    .pagination { display: flex; align-items: center; justify-content: center; gap: var(--space-5); }
    .empty-state { display: flex; flex-direction: column; align-items: center; justify-content: center; padding: var(--space-10) var(--space-5); text-align: center; }

    @media (max-width: 640px) {
      .search-bar { width: 100%; }
      .filter-select { width: 100%; }
      .top-controls { width: 100%; }
    }
  `]
})
export class AdminUsersComponent implements OnInit {
  private adminService = inject(AdminService);
  private toast = inject(ToastService);

  users: AdminUserListItem[] = [];
  isLoading = true;
  errorMessage = '';
  search = '';
  roleFilter = '';
  page = 1;
  limit = 20;
  totalItems = 0;
  totalPages = 1;
  private debounceTimer: any;

  ngOnInit() { this.loadUsers(); }

  onSearch() {
    clearTimeout(this.debounceTimer);
    this.debounceTimer = setTimeout(() => { this.page = 1; this.loadUsers(); }, 400);
  }

  onFilterChange() {
    this.page = 1;
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    this.errorMessage = '';
    this.adminService.getUsers({
      search: this.search || undefined,
      role: this.roleFilter || undefined,
      page: this.page,
      limit: this.limit,
    })
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          const data = res.data;
          if (data) {
            this.users = data.users ?? [];
            this.totalItems = data.totalItems ?? 0;
            this.totalPages = data.totalPages ?? 1;
          } else {
            this.users = [];
          }
        },
        error: () => {
          this.errorMessage = 'Failed to load users. Please try again.';
          this.toast.error('Failed to load users');
        },
      });
  }

  goToPage(p: number) { this.page = p; this.loadUsers(); }

  updateStatus(userId: string, status: number) {
    const numStatus = Number(status);
    this.adminService.updateUserStatus(userId, { status: numStatus }).subscribe({
      next: () => this.toast.success('User status updated'),
      error: () => this.toast.error('Failed to update status'),
    });
  }

  deleteUser(id: string) {
    if (!confirm('Are you sure you want to delete this user?')) return;
    this.adminService.deleteUser(id).subscribe({
      next: () => { this.toast.success('User deleted'); this.loadUsers(); },
      error: () => this.toast.error('Failed to delete user'),
    });
  }
}
