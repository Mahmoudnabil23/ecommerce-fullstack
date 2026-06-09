import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { AdminService } from '../../../core/services/admin.service';
import { CategoryItem } from '../../../core/models/category.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, LoadingSpinnerComponent],
  template: `
    <div>
      <div class="page-top">
        <h1 class="heading-3">Categories</h1>
        <span class="caption text-muted" *ngIf="flatCategories.length > 0">{{ flatCategories.length }} categories</span>
      </div>

      @if (isLoading) {
        <app-loading-spinner />
      } @else if (errorMessage) {
        <div class="empty-state card">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#ef4444" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>
          <p class="body-small text-muted" style="margin-top: var(--space-4);">{{ errorMessage }}</p>
          <button class="btn btn-ghost btn-sm" style="margin-top: var(--space-4);" (click)="loadCategories()">Retry</button>
        </div>
      } @else if (flatCategories.length === 0) {
        <div class="empty-state card">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color: var(--color-shade-50);"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>
          <p class="body-small text-muted" style="margin-top: var(--space-4);">No categories found</p>
        </div>
      } @else {
        <div class="table-card card">
          <table class="admin-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Slug</th>
                <th>Parent</th>
                <th>Image</th>
              </tr>
            </thead>
            <tbody>
              @for (cat of flatCategories; track cat.id) {
                <tr>
                  <td class="body-small">
                    @if (cat.depth > 0) {
                      <span class="indent-marker" [style.padding-left.px]="cat.depth * 20">↳ </span>
                    }
                    {{ cat.name }}
                  </td>
                  <td class="caption text-muted">{{ cat.slug }}</td>
                  <td class="caption text-muted">{{ cat.parentName ?? '—' }}</td>
                  <td>
                    @if (cat.imageUrl) {
                      <img [src]="cat.imageUrl" [alt]="cat.name" class="cat-thumb" />
                    } @else {
                      <span class="caption text-muted">—</span>
                    }
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
  styles: [`
    .page-top { display: flex; justify-content: space-between; align-items: center; gap: var(--space-4); margin-bottom: var(--space-7); flex-wrap: wrap; }
    .table-card { overflow-x: auto; }
    .admin-table { width: 100%; border-collapse: collapse; }
    .admin-table th { text-align: left; padding: var(--space-4) var(--space-5); font-size: 12px; font-weight: 500; color: var(--color-muted); text-transform: uppercase; letter-spacing: 0.72px; border-bottom: 1px solid var(--color-shade-70); }
    .admin-table td { padding: var(--space-4) var(--space-5); border-bottom: 1px solid var(--color-dark-card-border); vertical-align: middle; }
    .admin-table tr:hover td { background: rgba(255,255,255,0.02); }
    .empty-state { display: flex; flex-direction: column; align-items: center; justify-content: center; padding: var(--space-10) var(--space-5); text-align: center; }
    .indent-marker { color: var(--color-shade-50); }
    .cat-thumb { width: 36px; height: 36px; border-radius: var(--radius-sm); object-fit: cover; border: 1px solid var(--color-dark-card-border); }
  `]
})
export class AdminCategoriesComponent implements OnInit {
  private adminService = inject(AdminService);

  flatCategories: FlatCategory[] = [];
  isLoading = true;
  errorMessage = '';

  ngOnInit() { this.loadCategories(); }

  loadCategories() {
    this.isLoading = true;
    this.errorMessage = '';
    this.adminService.getCategories()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          const tree = res.data ?? [];
          this.flatCategories = this.flattenTree(tree, null, 0);
        },
        error: () => {
          this.errorMessage = 'Failed to load categories. Please try again.';
        },
      });
  }

  private flattenTree(nodes: CategoryItem[], parentName: string | null, depth: number): FlatCategory[] {
    const result: FlatCategory[] = [];
    for (const node of nodes) {
      result.push({
        id: node.id,
        name: node.name,
        slug: node.slug,
        imageUrl: node.imageUrl,
        parentName,
        depth,
      });
      if (node.children?.length) {
        result.push(...this.flattenTree(node.children, node.name, depth + 1));
      }
    }
    return result;
  }
}

interface FlatCategory {
  id: string;
  name: string;
  slug: string;
  imageUrl?: string;
  parentName: string | null;
  depth: number;
}
