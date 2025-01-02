import { Injectable } from '@angular/core';
import { HttpService } from '../http/http.service';
import { environment } from '../../../environments/environment';
import { ConfirmOrderPayload, DiscountUsagePayload, OrderPayload } from '../../models/request/order';
import { Observable } from 'rxjs';
import { OrderCartDTO, OrderDTO, OrderList } from '../../models/response/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  
  baseUrl = environment.orderServiceDomain;

  constructor(private apiService: HttpService) { }

  addToCart(payload: OrderPayload): Observable<string | null>{
    var apiUrl = `${this.baseUrl}/api/order/add-to-cart`;
    return this.apiService.postData<string, OrderPayload>(apiUrl, payload);
  }

  getCart(): Observable<OrderCartDTO>{
    var apiUrl = `${this.baseUrl}/api/order/get-cart`;
    return this.apiService.getData<OrderCartDTO>(apiUrl);
  }

  confirmOrder(payload: ConfirmOrderPayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/order/confirm-order`;
    return this.apiService.postData<boolean, ConfirmOrderPayload>(apiUrl, payload);
  }

  cancelOrder(orderId: string): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/order/cancel-order/${orderId}`;
    return this.apiService.deleteData<boolean>(apiUrl);
  }

  viewOrder(orderId: string): Observable<OrderDTO>{
    var apiUrl = `${this.baseUrl}/api/order/view-order/${orderId}`;
    return this.apiService.getData<OrderDTO>(apiUrl);
  }

  viewAllOrders(pageSize:number, pageNumber: number): Observable<OrderList>{
    var apiUrl = `${this.baseUrl}/api/order/view-all-orders?pageSize=${pageSize}&pageNumber=${pageNumber}`;
    return this.apiService.getData<OrderList>(apiUrl);
  }

  applyDiscount(payload: DiscountUsagePayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/discount/apply`;
    return this.apiService.postData<boolean, DiscountUsagePayload>(apiUrl, payload);
  }

  cancelDiscount(payload: DiscountUsagePayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/discount/cancel`;
    return this.apiService.postData<boolean, DiscountUsagePayload>(apiUrl, payload);
  }

}
