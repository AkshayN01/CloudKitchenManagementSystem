import { Component, OnInit } from '@angular/core';
import { LoginPayload } from '../../models/request/customer';
import { ProfileService } from '../../services/user-profile/profile.service';
import { FormBuilder } from '@angular/forms';
import { SessionService } from '../../services/session/session.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  public loginValid = true;
  public loginRequest: LoginPayload = { userName : "", password: "" };

  constructor(private fb :FormBuilder, private userAccountService: ProfileService, private sessionService: SessionService, private router: Router)
  {}

  ngOnInit(): void {
    var isExpired = this.sessionService.isTokenExpired();
    if(!isExpired){
      this.router.navigate(["/home"]);
    }
  }

  public onSubmit(): void {
    this.loginValid = true;
    this.userAccountService.login(this.loginRequest).subscribe((response) => {
      this.sessionService.saveDetails(response);
      this.router.navigate(["/home"]);
    })
  }
}
