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

  private loginAPIUrl = environment.loginAPIUrl;
  private registerAPIUrl = environment.registerAPIUrl;

  constructor(private apiService: HttpService) { }

  login(payload: LoginRequest): Observable<LoginResponse>{
    return this.apiService.postData<LoginResponse, LoginRequest>(this.loginAPIUrl, payload);
  }

  register()
}