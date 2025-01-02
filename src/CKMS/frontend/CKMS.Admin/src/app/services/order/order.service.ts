import { Injectable } from '@angular/core';
import { HttpService } from '../http/http.service';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { OrderDetail, OrderList } from '../../models/response/orders/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  baseUrl = environment.orderServiceDomain;

  constructor(private apiService: HttpService) { }

  updateOrder(status: string, orderId: string): Observable<boolean>{
    var apiUrl = this.baseUrl + "/api/order/kitchen/update-order/" + orderId
    return this.apiService.putData<boolean, string>(apiUrl, status);
  }

  viewOrder(orderId:string): Observable<OrderDetail>{
    var apiUrl = this.baseUrl + "/api/order/kitchen/view-order?orderId=" + orderId
    return this.apiService.getData<OrderDetail>(apiUrl);
  }

  viewAllOrders(pageSize: number, pageNumber: number, staus: string): Observable<OrderList>{
    console.log(staus == '')
    var apiUrl = `${this.baseUrl}/api/order/kitchen/view-all-orders?pageSize=${pageSize}&pageNumber=${pageNumber}`;
    if(staus != null && staus != undefined && staus != '')
      apiUrl += `&status=${staus}`;
    return this.apiService.getData<OrderList>(apiUrl);
  }
}
