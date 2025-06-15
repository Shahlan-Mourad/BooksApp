import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';
import { firstValueFrom } from 'rxjs';
import { Book, CreateBookRequest, UpdateBookRequest, ApiResponse } from '../models';

// Backend Book interface
interface BackendBook {
  id: number;
  title: string;
  author: string;
  isbn: string;
  publishedDate: string; // This comes as ISO string from backend
  description: string;
  coverImageUrl?: string;
  addedByUsername: string;
  createdAt: string;
  updatedAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private readonly API_URL = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getHeaders(): HttpHeaders {
    const authHeaders = this.authService.getAuthHeaders();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      ...authHeaders
    });
  }

  private convertBackendBookToFrontend(backendBook: BackendBook): Book {
    return {
      id: backendBook.id,
      title: backendBook.title,
      author: backendBook.author,
      isbn: backendBook.isbn,
      publishedYear: new Date(backendBook.publishedDate).getFullYear(),
      description: backendBook.description,
      coverImageUrl: backendBook.coverImageUrl,
      isFavorite: false // Default to false, will be updated when fetching favorites
    };
  }

  private convertFrontendBookToBackend(book: CreateBookRequest | UpdateBookRequest): any {
    return {
      title: book.title,
      author: book.author,
      isbn: book.isbn,
      publishedDate: new Date(book.publishedYear, 0, 1).toISOString(),
      description: book.description,
      coverImageUrl: null // Add this field
    };
  }

  async getBooks(): Promise<ApiResponse<Book[]>> {
    try {
      const response = await firstValueFrom(
        this.http.get<BackendBook[]>(`${this.API_URL}/books`, {
          headers: this.getHeaders()
        })
      );

      if (!response) {
        return { success: true, data: [] };
      }

      if (!Array.isArray(response)) {
        return { success: false, message: 'Invalid response format from server' };
      }

      const books = response.map(book => this.convertBackendBookToFrontend(book));
      return { success: true, data: books };
    } catch (error: any) {
      console.error('Get books error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to load books' 
      };
    }
  }

  async getBook(id: number): Promise<ApiResponse<Book>> {
    try {
      const response = await firstValueFrom(
        this.http.get<BackendBook>(`${this.API_URL}/books/${id}`, {
          headers: this.getHeaders()
        })
      );

      if (response) {
        const book = this.convertBackendBookToFrontend(response);
        return { success: true, data: book };
      }
      return { success: false, message: 'Book not found' };
    } catch (error: any) {
      console.error('Get book error:', error);
      console.error('Error status:', error.status);
      console.error('Error message:', error.message);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to load book' 
      };
    }
  }

  async addBook(book: CreateBookRequest): Promise<ApiResponse<Book>> {
    try {
      const backendBook = this.convertFrontendBookToBackend(book);
      const response = await firstValueFrom(
        this.http.post<BackendBook>(`${this.API_URL}/books`, backendBook, {
          headers: this.getHeaders()
        })
      );

      if (response) {
        const frontendBook = this.convertBackendBookToFrontend(response);
        return { success: true, data: frontendBook };
      }
      return { success: false, message: 'Failed to create book' };
    } catch (error: any) {
      console.error('Add book error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to add book' 
      };
    }
  }

  async updateBook(id: number, book: UpdateBookRequest): Promise<ApiResponse<Book>> {
    try {
      const backendBook = this.convertFrontendBookToBackend(book);
      const response = await firstValueFrom(
        this.http.put<BackendBook>(`${this.API_URL}/books/${id}`, backendBook, {
          headers: this.getHeaders()
        })
      );

      if (response) {
        const frontendBook = this.convertBackendBookToFrontend(response);
        return { success: true, data: frontendBook };
      }
      return { success: false, message: 'Failed to update book' };
    } catch (error: any) {
      return { 
        success: false, 
        message: error.error?.message || 'Failed to update book' 
      };
    }
  }

  async deleteBook(id: number): Promise<ApiResponse<void>> {
    try {
      await firstValueFrom(
        this.http.delete(`${this.API_URL}/books/${id}`, {
          headers: this.getHeaders()
        })
      );

      return { success: true };
    } catch (error: any) {
      console.error('Delete book error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to delete book' 
      };
    }
  }

  async addToFavorites(bookId: number): Promise<ApiResponse<void>> {
    try {
      await firstValueFrom(
        this.http.post(`${this.API_URL}/userfavoritebooks`, {
          bookId: bookId
        }, {
          headers: this.getHeaders()
        })
      );

      return { success: true };
    } catch (error: any) {
      console.error('Add to favorites error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to add to favorites' 
      };
    }
  }

  async removeFromFavorites(bookId: number): Promise<ApiResponse<void>> {
    try {
      await firstValueFrom(
        this.http.delete(`${this.API_URL}/userfavoritebooks/${bookId}`, {
          headers: this.getHeaders()
        })
      );

      return { success: true };
    } catch (error: any) {
      console.error('Remove from favorites error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to remove from favorites' 
      };
    }
  }

  async getFavoriteBooks(): Promise<ApiResponse<Book[]>> {
    try {
      const response = await firstValueFrom(
        this.http.get<BackendBook[]>(`${this.API_URL}/userfavoritebooks`, {
          headers: this.getHeaders()
        })
      );

      if (!response) {
        return { success: true, data: [] };
      }

      const books = response.map(book => ({
        ...this.convertBackendBookToFrontend(book),
        isFavorite: true
      }));
      return { success: true, data: books };
    } catch (error: any) {
      console.error('Get favorite books error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to load favorite books' 
      };
    }
  }

  async searchBooks(query: string): Promise<ApiResponse<Book[]>> {
    try {
      const response = await firstValueFrom(
        this.http.get<BackendBook[]>(`${this.API_URL}/books/search?q=${encodeURIComponent(query)}`, {
          headers: this.getHeaders()
        })
      );

      if (!response) {
        return { success: true, data: [] };
      }

      const books = response.map(book => this.convertBackendBookToFrontend(book));
      return { success: true, data: books };
    } catch (error: any) {
      console.error('Search books error:', error);
      return { 
        success: false, 
        message: error.error?.message || 'Failed to search books' 
      };
    }
  }
} 