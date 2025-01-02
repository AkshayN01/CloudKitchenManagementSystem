import { Injectable } from '@angular/core';
import { HttpService } from '../http/http.service';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { BestSellingDish, CustomerSummary, OrderReportSummary, TopCustomers } from '../../models/response/orders/report';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  baseUrl: string = environment.orderServiceDomain;

  constructor(private apiService: HttpService) { }

  getOrderReportSummary(startDate:string, endDate:string):Observable<OrderReportSummary>{
    var apiUrl = `${this.baseUrl}/api/report/get-summary?startDate=${startDate}&endDate=${endDate}`;
    return this.apiService.getData<OrderReportSummary>(apiUrl);
  }

  getBestSellingDishes(startDate:string, endDate:string, top: number, desc: boolean): Observable<BestSellingDish[]>{
    var apiUrl = `${this.baseUrl}/api/report/best-selling-dish?startDate=${startDate}&endDate=${endDate}&top=${top}&desc=${desc}`;
    return this.apiService.getData<BestSellingDish[]>(apiUrl)
  }

  getTopCustomers(startDate:string, endDate:string, pageSize: number, pageNumber: number, desc: boolean): Observable<TopCustomers[]>{
    var apiUrl = `${this.baseUrl}/api/report/top-customers?dec=${desc}&pageSize=${pageSize}`
    if(startDate != '' && startDate != undefined)
      apiUrl += `&startDate=${startDate}&endDate=${endDate}`;
    else if(pageNumber != 0)
      apiUrl += `&pageNumber=${pageNumber}`;

    return this.apiService.getData<TopCustomers[]>(apiUrl);
  }

  getCustomerSummary(customerId: string): Observable<CustomerSummary>{
    var apiUrl = `${this.baseUrl}/api/report/customer-summary?customerId=${customerId}`;
    return this.apiService.getData<CustomerSummary>(apiUrl);
  }
}
