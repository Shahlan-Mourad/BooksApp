import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, User } from '../models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  async getUserProfile(): Promise<ApiResponse<User>> {
    try {
      const response = await firstValueFrom(this.http.get<User>(`${this.API_URL}/users/profile`));
      return { success: true, data: response };
    } catch (error: any) {
      console.error('Error fetching user profile:', error);
      return { success: false, message: error.error?.message || 'Failed to fetch user profile.' };
    }
  }

  async updateUserProfile(user: User): Promise<ApiResponse<User>> {
    try {
      const response = await firstValueFrom(this.http.put<User>(`${this.API_URL}/users/profile`, user));
      return { success: true, data: response };
    } catch (error: any) {
      console.error('Error updating user profile:', error);
      return { success: false, message: error.error?.message || 'Failed to update user profile.' };
    }
  }
} 