import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, AuthResponse, ApiResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private sessionTimeout: any = null;
  private readonly SESSION_TIMEOUT_MINUTES = 30; // 30 minutes session timeout

  constructor(private http: HttpClient, private ngZone: NgZone) {
    // Load stored user data on service initialization
     this.loadStoredUser();
    
    // Setup session timeout
    this.setupSessionTimeout();
    
    // Setup auto-logout when browser is closed
    this.setupAutoLogout();

   
  }

  private setupSessionTimeout(): void {
    // Reset session timeout on user activity
    const resetSessionTimeout = () => {
      if (this.sessionTimeout) {
        clearTimeout(this.sessionTimeout);
      }
      
      if (this.isAuthenticated()) {
        this.sessionTimeout = setTimeout(() => {
          this.logout();
        }, this.SESSION_TIMEOUT_MINUTES * 60 * 1000);
      }
    };

    // Listen for user activity
    ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart'].forEach(event => {
      document.addEventListener(event, resetSessionTimeout, true);
    });

    // Initial setup
    resetSessionTimeout();
  }

  private startSessionTimeout(): void {
    if (this.sessionTimeout) {
      clearTimeout(this.sessionTimeout);
    }
    
    this.sessionTimeout = setTimeout(() => {
      this.logout();
    }, this.SESSION_TIMEOUT_MINUTES * 60 * 1000);
  }

  private loadStoredUser(): void {
    const token = this.getToken();
    const user = this.getStoredUser();
    
    if (token && user) {
      // Check if token is expired
      if (this.isTokenExpired(token)) {
        this.removeToken();
        this.removeStoredUser();
        this.currentUserSubject.next(null);
        return;
      }
      
      // Validate that user data is complete
      if (user.id && user.username && user.email) {
        this.currentUserSubject.next(user);
        // Start session timeout for existing session
        this.startSessionTimeout();
      } else {
        this.removeToken();
        this.removeStoredUser();
        this.currentUserSubject.next(null);
      }
    } else {
      this.currentUserSubject.next(null);
    }
  }

  async login(email: string, password: string): Promise<ApiResponse<any>> {
    try {
      const response = await this.http.post<AuthResponse>(`${this.API_URL}/auth/login`, {
        username: email,
        password
      }).toPromise();

      if (response?.token) {
        this.setToken(response.token);
        const user = {
          id: response.id,
          username: response.username,
          email: response.email,
          firstName: response.firstName,
          lastName: response.lastName
        };
        this.setStoredUser(user);
        this.currentUserSubject.next(user);
        
        // Start session timeout after successful login
        this.startSessionTimeout();
        
        return { success: true, data: user };
      } else {
        return { success: false, message: 'Login failed - no token received' };
      }
    } catch (error: any) {
      let errorMessage = 'An error occurred during login';
      
      if (error.status === 401) {
        errorMessage = 'Invalid email or password. Please try again.';
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      }
      
      return { 
        success: false, 
        message: errorMessage
      };
    }
  }

  async register(firstName: string, lastName: string, email: string, password: string): Promise<ApiResponse<any>> {
    try {
      const response = await firstValueFrom(
        this.http.post<AuthResponse>(`${this.API_URL}/auth/register`, {
          username: email,
          email,
          firstName,
          lastName,
          password,
          confirmPassword: password
        })
      );

      if (response?.token) {
        return { success: true, message: 'Registration successful' };
      } else {
        return { success: false, message: 'Registration failed' };
      }
    } catch (error: any) {
      console.error('Register error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'An error occurred during registration' 
      };
    }
  }

  logout(): void {
    // Clear session timeout
    if (this.sessionTimeout) {
      clearTimeout(this.sessionTimeout);
      this.sessionTimeout = null;
    }
    
    this.removeToken();
    this.removeStoredUser();
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(token);
  }

  getCurrentUser(): any {
    return this.currentUserSubject.value;
  }

  private isTokenExpired(token: string): boolean {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) {
        return true;
      }
      
      const payload = JSON.parse(atob(parts[1]));
      const expirationDate = new Date(payload.exp * 1000);
      const isExpired = expirationDate < new Date();
      
      return isExpired;
    } catch (error) {
      return true;
    }
  }

  private setToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  private getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  private removeToken(): void {
    localStorage.removeItem('auth_token');
  }

  private setStoredUser(user: any): void {
    localStorage.setItem('current_user', JSON.stringify(user));
  }

  private getStoredUser(): any {
    const user = localStorage.getItem('current_user');
    return user ? JSON.parse(user) : null;
  }

  private removeStoredUser(): void {
    localStorage.removeItem('current_user');
  }

  // Get auth headers for API requests
  getAuthHeaders(): { [key: string]: string } {
    const token = this.getToken();
    return token ? { 'Authorization': `Bearer ${token}` } : {};
  }

  // Method to cleanup event listeners (call this when app is destroyed)
  cleanup(): void {
    // Clear session timeout
    if (this.sessionTimeout) {
      clearTimeout(this.sessionTimeout);
      this.sessionTimeout = null;
    }
    
    // Remove event listeners (these are automatically cleaned up by the browser)
    // No need to manually remove them as they're bound to window events
    
    // Clear all auth data
    this.logout();
  }

  private setupAutoLogout(): void {
    // Check on page load if we should clear auth data
    document.addEventListener('DOMContentLoaded', () => {
      // Check if this is a refresh or a fresh page load
      const isRefresh = performance.navigation.type === 1; // 1 = refresh
      
      // If this is NOT a refresh and user is authenticated, clear auth
      if (!isRefresh && this.isAuthenticated()) {
        // This was a browser close/reopen, not a refresh
        // Use NgZone to ensure proper change detection
        this.ngZone.run(() => {
          this.logout();
          // Force a change detection cycle
          setTimeout(() => {
            this.currentUserSubject.next(null);
          }, 0);
        });
      }
    });
  }

  
}