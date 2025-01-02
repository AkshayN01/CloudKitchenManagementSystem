import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProfileService } from '../../services/user-profile/profile.service';

@Component({
  selector: 'app-verify-account',
  templateUrl: './verify-account.component.html',
  styleUrl: './verify-account.component.css'
})
export class VerifyAccountComponent implements OnInit{
  message = 'Verifying your account...';
  loading = true;
  verificationSuccess = false;

  constructor(private route: ActivatedRoute, private userAccountService: ProfileService, private router: Router) {}

  ngOnInit() {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (token) {
      this.userAccountService.verifyAccount(token).subscribe((res) => {
        if(res){
          this.message = 'Account verified successfully!';
          setTimeout(()=>{
            this.router.navigate(['/login']);
          },3000);
        }
        else
          this.message = 'Verification failed. Please try again.';
      });
    } else {
      this.message = 'Invalid or missing token.';
      this.verificationSuccess = false;
      this.loading = false;
    }
    
  }

}
