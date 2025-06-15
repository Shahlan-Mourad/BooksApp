import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { AuthService } from '../../../core';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerData = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  };
  errorMessage = '';
  successMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  async onSubmit() {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    // Validate passwords match
    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      this.isLoading = false;
      this.cdr.detectChanges();
      return;
    }

    // Validate password strength
    if (this.registerData.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters long';
      this.isLoading = false;
      this.cdr.detectChanges();
      return;
    }

    try {
      const result = await this.authService.register(
        this.registerData.firstName,
        this.registerData.lastName,
        this.registerData.email,
        this.registerData.password
      );
      
      if (result.success) {
        // Auto-login after successful registration
        try {
          const loginResult = await this.authService.login(
            this.registerData.email,
            this.registerData.password
          );
          
          if (loginResult.success) {
            this.successMessage = `Welcome ${this.registerData.firstName}! Your account has been created successfully.`;
            setTimeout(() => {
              this.router.navigate(['/books']);
            }, 2000);
          } else {
            this.errorMessage = 'Account created but automatic login failed. Please log in manually.';
            this.isLoading = false;
            this.cdr.detectChanges();
            setTimeout(() => {
              this.router.navigate(['/login']);
            }, 3000);
          }
        } catch (loginError) {
          this.errorMessage = 'Account created but automatic login failed. Please log in manually.';
          this.isLoading = false;
          this.cdr.detectChanges();
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 3000);
        }
      } else {
        // Handle specific error messages
        if (result.message && (result.message.toLowerCase().includes('username') || 
            result.message.toLowerCase().includes('username'))) {
          this.errorMessage = 'This email is already registered. Please use a different email or try logging in.';
        } else {
          this.errorMessage = result.message || 'Registration failed. Please try again.';
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    } catch (error: any) {
      // Handle network or server errors
      if (error.error && error.error.message) {
        if (error.error.message.toLowerCase().includes('username') || 
            error.error.message.toLowerCase().includes('username')) {
          this.errorMessage = 'This email is already registered. Please use a different email or try logging in.';
        } else {
          this.errorMessage = error.error.message;
        }
      } else {
        this.errorMessage = 'An error occurred during registration. Please try again.';
      }
      this.isLoading = false;
      this.cdr.detectChanges();
      console.error('Registration error:', error);
    }
  }

  clearError() {
    this.errorMessage = '';
  }

  getUniqueYears(): number[] {
    // Implementation of getUniqueYears method
    return [];
  }
} 