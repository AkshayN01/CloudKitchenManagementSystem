import { Injectable } from '@angular/core';
import { LoginResponse } from '../../models/response/customer';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  authStorageName: string;
  nameStorageName: string = 'AdminName';
  loginData!: LoginResponse;

  constructor(private jwtHelper: JwtHelperService) { 
    this.authStorageName = environment.authStorageName;
  }

  saveDetails = (details: LoginResponse) => {
    localStorage.setItem(this.nameStorageName, details.name);
    localStorage.setItem(this.authStorageName, details.token);
  }

  getToken = (): string | null => {
    return localStorage.getItem(this.authStorageName);
  }

  getUserName = (): string | null => {
    return localStorage.getItem(this.nameStorageName);
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    return this.jwtHelper.isTokenExpired(token);
  }

  getTokenExpirationDate(): Date | null {
    const token = this.getToken();
    if(token == null)
      return null;
    return this.jwtHelper.getTokenExpirationDate(token);
  }
  
  deleteDetails = () => {
    localStorage.removeItem(this.authStorageName);
  }

  logout = () => {
    
  }
}
