import { Injectable } from '@angular/core';
import { LoginResponse } from '../../models/response/login/login';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  authStorageName: string;
  nameStorageName: string = 'AdminName';
  kitchenStorageName: string = 'KitchenName';
  loginData!: LoginResponse;

  constructor(private jwtHelper: JwtHelperService) { 
    this.authStorageName = environment.authStorageName;
  }

  saveDetails = (details: LoginResponse) => {
    localStorage.setItem(this.nameStorageName, details.name);
    localStorage.setItem(this.authStorageName, details.token);
    localStorage.setItem(this.kitchenStorageName, details.kitchenName);
  }

  getToken = (): string | null => {
    return localStorage.getItem(this.authStorageName);
  }

  getUserName = (): string | null => {
    return localStorage.getItem(this.nameStorageName);
  }

  getKitchenName = (): string | null => {
    return localStorage.getItem(this.kitchenStorageName);
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
    localStorage.removeItem(this.nameStorageName);
    localStorage.removeItem(this.kitchenStorageName);
  }

  logout = () => {
    this.deleteDetails();
  }
}
