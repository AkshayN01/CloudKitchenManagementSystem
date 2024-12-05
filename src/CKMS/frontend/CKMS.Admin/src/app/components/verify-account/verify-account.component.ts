import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserAccountService } from '../../services/user-account/user-account.service';

@Component({
  selector: 'app-verify-account',
  templateUrl: './verify-account.component.html',
  styleUrl: './verify-account.component.css'
})
export class VerifyAccountComponent implements OnInit{
  message = 'Verifying your account...';
  loading = true;
  verificationSuccess = false;

  constructor(private route: ActivatedRoute, private userAccountService: UserAccountService) {}

  ngOnInit() {
    const token = this.route.snapshot.queryParamMap.get('token');
    setTimeout(() => {
      if (token) {
        this.userAccountService.verifyAccount(token).subscribe({
          next: () => (this.message = 'Account verified successfully!'),
          error: () => (this.message = 'Verification failed. Please try again.'),
        });
      } else {
        this.message = 'Invalid or missing token.';
        this.verificationSuccess = false;
        this.loading = false;
      }
    }, 5000);
    
  }

}
