import { Component, OnInit } from '@angular/core';
import { LoginRequest } from '../../models/request/login/login';
import { FormBuilder } from '@angular/forms';
import { UserAccountService } from '../../services/user-account/user-account.service';
import { SessionService } from '../../services/session/session.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  public loginValid = true;
  public loginRequest: LoginRequest = { userName : "", password: "" };

  constructor(private fb :FormBuilder, private userAccountService: UserAccountService, private sessionService: SessionService, private router: Router)
  {}

  ngOnInit(): void {
    var isExpired = this.sessionService.isTokenExpired();
    if(!isExpired){
      this.router.navigate(["/orders"]);
    }
  }

  public onSubmit(): void {
    this.loginValid = true;
    this.userAccountService.login(this.loginRequest).subscribe((response) => {
      this.sessionService.saveDetails(response);
      this.router.navigate(["/orders"]);
    })
  }
}
