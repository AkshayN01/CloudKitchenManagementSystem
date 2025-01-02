import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserAccountService } from '../../services/user-account/user-account.service';
import { Router } from '@angular/router';
import { UtilityService } from '../../services/utility/utility.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  
  constructor(private fb : FormBuilder, private userAccountService: UserAccountService, private router: Router, private utilityService: UtilityService){
    this.registerForm = this.fb.group({
      kitchenName: ['', Validators.required],
      address: ['', Validators.required],
      city: ['', Validators.required],
      region: ['', Validators.required],
      postalCode: ['', Validators.required],
      country: ['', Validators.required],
      adminUserPayload: this.fb.group({
        emailId: ['', [Validators.required, Validators.email]],
        fullName: ['', Validators.required],
        password: ['', Validators.required],
        confirmPassword: ['', Validators.required]
      }, { validator: this.passwordMatchValidator })
    });
  }

  passwordMatchValidator(group: AbstractControl): { [key: string]: boolean } | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(){
    if(!this.registerForm.invalid){
      this.userAccountService.registerKitchen(this.registerForm.value).subscribe((res) => {
        if(res){
          this.utilityService.openSnackBar("A verification email will be sent to your email.");
          setTimeout(()=>{
            this.router.navigate(['/login']);
          },3000);
        }
      })
    }
  }
  
  resetForm() {
    this.registerForm.reset();
  }
}
