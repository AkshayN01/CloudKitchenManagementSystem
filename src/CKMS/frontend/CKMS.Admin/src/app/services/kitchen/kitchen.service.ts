import { Injectable } from '@angular/core';
import { HttpService } from '../http/http.service';
import { RegisterPayload } from '../../models/request/login/register';
import { Observable } from 'rxjs';
import { HTTPResponse } from '../../models/api-response/api-response';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class KitchenService {
  baseUrl: string = environment.apiUrl;

  constructor(private apiService: HttpService) { }

  register(payload: RegisterPayload): Observable<boolean>{
    var apiUrl = this.baseUrl + '/register-kitchen';
    return this.apiService.postData<boolean, RegisterPayload>(apiUrl, payload);
  };
}
