import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from '../http/http.service';
import { CustomerUpdatePayload, LoginPayload, RegisterPayload } from '../../models/request/customer';
import { Observable } from 'rxjs';
import { CustomerDTO } from '../../models/response/customer';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  baseUrl = environment.customerServiceDomain;

  constructor(private apiService: HttpService) { }

  register(payload: RegisterPayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/register`;
    return this.apiService.postData<boolean, RegisterPayload>(apiUrl, payload);
  }

  login(payload: LoginPayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/login`;
    return this.apiService.postData<boolean, LoginPayload>(apiUrl, payload);
  }

  verifyAccount(token: string): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/verify-user?token=${token}`;
    return this.apiService.getData<boolean>(apiUrl);
  }

  getUserDetails(): Observable<CustomerDTO>{
    var apiUrl = `${this.baseUrl}/api/customer/get-user`;
    return this.apiService.getData<CustomerDTO>(apiUrl);
  }

  updateCustomerDetails(payload: CustomerUpdatePayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/update-user`;
    return this.apiService.putData<boolean, CustomerUpdatePayload>(apiUrl, payload);
  }

  deleteUser(): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/delete-user`;
    return this.apiService.deleteData<boolean>(apiUrl);
  }
}
