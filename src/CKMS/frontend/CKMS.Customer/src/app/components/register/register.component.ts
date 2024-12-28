import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProfileService } from '../../services/user-profile/profile.service';
import { UtilityService } from '../../services/utility/utility.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm!: FormGroup;

  constructor(private fb: FormBuilder, private profileService: ProfileService, private router: Router, private utilityService: UtilityService){
    this.registerForm = this.fb.group({
      name:  ['', Validators.required],
      phoneNumber:  ['', Validators.required],
      emailId: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    },{ validator: this.passwordMatchValidator });
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
      this.profileService.register(this.registerForm.value).subscribe((res) => {
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
