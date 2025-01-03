import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from '../http/http.service';
import { Observable } from 'rxjs';
import { LoginRequest } from '../../models/request/login/login';
import { LoginResponse } from '../../models/response/login/login';

@Injectable({
  providedIn: 'root'
})
export class UserAccountService {
  
  private baseUrl: string = environment.adminServiceDomain;
  private loginAPIUrl = environment.loginAPIUrl;

  constructor(private apiService: HttpService) { }

  login(payload: LoginRequest): Observable<LoginResponse>{
    return this.apiService.postData<LoginResponse, LoginRequest>(this.loginAPIUrl, payload);
  }

  verifyAccount(token: string): Observable<boolean>{
    var apiUrl = this.baseUrl + '/api/admin/verify-user?'+ 'token='+token;
    return this.apiService.getData<boolean>(apiUrl);
  }

  registerKitchen(payload: any): Observable<boolean>{
    var apiUrl = this.baseUrl + '/api/admin/register-kitchen';
    return this.apiService.postData<boolean, any>(apiUrl, payload);
  }
}
