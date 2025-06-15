import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Quote, CreateQuoteRequest, UpdateQuoteRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class QuoteService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) { }

  async getQuotes(): Promise<ApiResponse<Quote[]>> {
    try {
      const response = await firstValueFrom(this.http.get<Quote[]>(`${this.API_URL}/quotes`));
      return { success: true, data: response };
    } catch (error: any) {
      console.error('Error fetching quotes:', error);
      return { success: false, message: error.error?.message || 'Failed to fetch quotes.' };
    }
  }

  async getQuote(id: number): Promise<ApiResponse<Quote>> {
    try {
      const response = await firstValueFrom(this.http.get<Quote>(`${this.API_URL}/quotes/${id}`));
      return { success: true, data: response };
    } catch (error: any) {
      console.error('Error fetching quote:', error);
      return { success: false, message: error.error?.message || 'Failed to fetch quote.' };
    }
  }

  async addQuote(quote: CreateQuoteRequest): Promise<ApiResponse<Quote>> {
    try {
      const response = await firstValueFrom(this.http.post<Quote>(`${this.API_URL}/quotes`, quote));
      return { success: true, data: response };
    } catch (error: any) {
      console.error('Error adding quote:', error);
      return { success: false, message: error.error?.message || 'Failed to add quote.' };
    }
  }

  async updateQuote(id: number, quote: UpdateQuoteRequest): Promise<ApiResponse<Quote>> {
    try {
      const response = await firstValueFrom(this.http.put<Quote>(`${this.API_URL}/quotes/${id}`, quote));
      return { success: true, data: response };
    } catch (error: any) {
      console.error('Error updating quote:', error);
      return { success: false, message: error.error?.message || 'Failed to update quote.' };
    }
  }

  async deleteQuote(id: number): Promise<ApiResponse<void>> {
    try {
      await firstValueFrom(this.http.delete<void>(`${this.API_URL}/quotes/${id}`));
      return { success: true };
    } catch (error: any) {
      console.error('Error deleting quote:', error);
      return { success: false, message: error.error?.message || 'Failed to delete quote.' };
    }
  }
} 