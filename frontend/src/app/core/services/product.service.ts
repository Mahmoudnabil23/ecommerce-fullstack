import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse, PaginatedResponse } from '../models/api-response.model';
import { ProductListItem, ProductDetail, ProductFilter } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/products`;

  getProducts(filter?: ProductFilter): Observable<ApiResponse<PaginatedResponse<ProductListItem>>> {
    let params = new HttpParams();
    if (filter) {
      if (filter.search) params = params.set('search', filter.search);
      if (filter.categoryId) params = params.set('categoryId', filter.categoryId);
      if (filter.minPrice != null) params = params.set('minPrice', filter.minPrice.toString());
      if (filter.maxPrice != null) params = params.set('maxPrice', filter.maxPrice.toString());
      if (filter.sellerId) params = params.set('sellerId', filter.sellerId);
      if (filter.inStock != null) params = params.set('inStock', filter.inStock.toString());
      if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
      if (filter.page) params = params.set('page', filter.page.toString());
      if (filter.limit) params = params.set('limit', filter.limit.toString());
    }
    return this.http.get<ApiResponse<PaginatedResponse<ProductListItem>>>(this.API, { params });
  }

  getProductById(id: string): Observable<ApiResponse<ProductDetail>> {
    return this.http.get<ApiResponse<ProductDetail>>(`${this.API}/${id}`);
  }

  getAvailableStock(id: string): Observable<ApiResponse<number>> {
    return this.http.get<ApiResponse<number>>(`${this.API}/${id}/stock`);
  }
}
