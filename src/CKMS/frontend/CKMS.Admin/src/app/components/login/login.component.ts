import { Component, OnInit } from '@angular/core';
import { LoginRequest } from '../../models/request/login/login';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent{
  public loginValid = true;
  public loginRequest: LoginRequest = { userName : "", password: "" };

  constructor(private fb :FormBuilder){}

  public onSubmit(): void {
    this.loginValid = true;
    
  }
}
