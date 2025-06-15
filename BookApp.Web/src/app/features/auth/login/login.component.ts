import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ChangeDetectorRef, NgZone } from '@angular/core';
import { AuthService } from '../../../core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginData = {
    email: '', // Use email for login (backend uses email as username)
    password: ''
  };
  errorMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}

  onSubmit() {
    this.isLoading = true;
    this.errorMessage = '';
    
    // Force change detection in NgZone
    this.ngZone.run(() => {
      this.cdr.detectChanges();
    });

    this.authService.login(this.loginData.email, this.loginData.password)
      .then(result => {
        if (result.success) {
          this.router.navigate(['/books']);
        } else {
          this.ngZone.run(() => {
            this.errorMessage = result.message || 'Login failed. Please try again.';
            this.isLoading = false;
            this.cdr.detectChanges();
          });
        }
      })
      .catch(error => {
        this.ngZone.run(() => {
          this.errorMessage = 'Invalid email or password. Please try again.';
          this.isLoading = false;
          this.cdr.detectChanges();
        });
      });
  }

  clearError() {
    this.errorMessage = '';
  }
} 