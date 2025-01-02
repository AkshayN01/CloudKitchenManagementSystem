import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from '../http/http.service';
import { Observable } from 'rxjs';
import { AddressPayload, AddressUpdatePayload } from '../../models/request/customer';
import { AddressDTO } from '../../models/response/customer';

@Injectable({
  providedIn: 'root'
})
export class AddressService {

  baseUrl = environment.customerServiceDomain;

  constructor(private apiService: HttpService) { }

  addAddress(payload: AddressPayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/add-address`
    return this.apiService.postData<boolean, AddressPayload>(apiUrl, payload);
  }
  
  updateAddress(payload: AddressUpdatePayload): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/update-address`
    return this.apiService.postData<boolean, AddressUpdatePayload>(apiUrl, payload);
  }

  deleteAddress(addressId: string): Observable<boolean>{
    var apiUrl = `${this.baseUrl}/api/customer/delete-address/${addressId}`
    return this.apiService.deleteData<boolean>(apiUrl);
  }

  getAddress(addressId: string): Observable<AddressDTO>{
    var apiUrl = `${this.baseUrl}/api/customer/get-address/${addressId}`
    return this.apiService.getData<AddressDTO>(apiUrl);
  }
  
  getAllAddress(): Observable<AddressDTO[]>{
    var apiUrl = `${this.baseUrl}/api/customer/get-all-address`
    return this.apiService.getData<AddressDTO[]>(apiUrl);
  }
}
