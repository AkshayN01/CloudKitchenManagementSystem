import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  
  constructor(private fb : FormBuilder){
    this.registerForm = this.fb.group({
      kitchenName: ['', Validators.required],
      address: ['', Validators.required],
      city: ['', Validators.required],
      region: ['', Validators.required],
      postalCode: ['', Validators.required],
      country: ['', Validators.required],
      adminUser: this.fb.group({
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

  }
  
  resetForm() {
    this.registerForm.reset();
  }
}
