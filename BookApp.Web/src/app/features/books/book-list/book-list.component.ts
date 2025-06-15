import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BookService, Book, CreateBookRequest, AuthService } from '../../../core';
import { BookCardComponent } from '../../../shared/components/book-card/book-card.component';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule, BookCardComponent],
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.scss']
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  filteredBooks: Book[] = [];
  searchTerm = '';
  selectedYear: number | null = null;
  isLoading = false;
  errorMessage = '';
  bookToDelete: Book | null = null;
  isSearchVisible = false;
  
  constructor(
    private bookService: BookService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  async ngOnInit() {
    // Wait a bit to ensure authentication is ready
    await this.delay(100);
    
    // Check if user is authenticated
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }
    
    await this.loadBooks();
  }

  private delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  async loadBooks() {
    this.isLoading = true;
    this.errorMessage = '';

    // Try up to 3 times with increasing delays
    for (let attempt = 1; attempt <= 3; attempt++) {
      try {
        const result = await this.bookService.getBooks();
        
        if (result.success) {
          this.books = result.data || [];
          this.filterBooks();
          
          // Small delay to ensure everything is ready
          await this.delay(50);
          this.cdr.detectChanges();
          
          break; // Success, exit the retry loop
        } else {
          this.errorMessage = result.message || 'Failed to load books';
          console.error(`Failed to load books (attempt ${attempt}/3):`, this.errorMessage);
          
          if (attempt < 3) {
            await this.delay(attempt * 500);
          }
        }
      } catch (error) {
        this.errorMessage = 'An error occurred while loading books';
        console.error(`Load books error (attempt ${attempt}/3):`, error);
        
        if (attempt < 3) {
          await this.delay(attempt * 500);
        }
      }
    }
    
    this.isLoading = false;
    this.cdr.detectChanges();
  }

  filterBooks() {
    this.filteredBooks = this.books.filter(book => {
      const matchesSearch = !this.searchTerm || 
        book.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        book.author.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        book.isbn.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesYear = !this.selectedYear || book.publishedYear === this.selectedYear;
      
      return matchesSearch && matchesYear;
    });
    this.cdr.detectChanges();
  }

  onSearchChange() {
    this.filterBooks();
    this.cdr.detectChanges();
  }

  onYearChange() {
    this.filterBooks();
    this.cdr.detectChanges();
  }

  clearFilters() {
    this.searchTerm = '';
    this.selectedYear = null;
    this.filterBooks();
    this.cdr.detectChanges();
  }

  async deleteBook(bookId: number) {
    // Find the book to delete
    this.bookToDelete = this.books.find(book => book.id === bookId) || null;
    
    if (!this.bookToDelete) return;

    // Show the Bootstrap modal instead of browser confirm
    const modal = document.getElementById('deleteConfirmModal');
    if (modal) {
      const bootstrapModal = new (window as any).bootstrap.Modal(modal);
      bootstrapModal.show();
    }
  }

  async confirmDelete() {
    if (!this.bookToDelete) return;

    this.isLoading = true;
    this.errorMessage = '';

    try {
      const result = await this.bookService.deleteBook(this.bookToDelete.id);
      if (result.success) {
        // Hide the modal
        const modal = document.getElementById('deleteConfirmModal');
        if (modal) {
          const bootstrapModal = (window as any).bootstrap.Modal.getInstance(modal);
          bootstrapModal.hide();
        }
        this.bookToDelete = null;
        await this.loadBooks();
      } else {
        this.errorMessage = result.message || 'Failed to delete book';
      }
    } catch (error) {
      this.errorMessage = 'An error occurred while deleting the book';
      console.error('Delete book error:', error);
    } finally {
      this.isLoading = false;
    }
  }

  addBook() {
    this.router.navigate(['/books/add']);
  }

  editBook(bookId: number) {
    this.router.navigate(['/books/edit', bookId]);
  }

  viewBook(bookId: number) {
    this.router.navigate(['/books', bookId]);
  }

  toggleSearch() {
    this.isSearchVisible = !this.isSearchVisible;
    if (this.isSearchVisible) {
      // Focus on search input after a short delay
      setTimeout(() => {
        const searchInput = document.getElementById('searchInput') as HTMLInputElement;
        if (searchInput) {
          searchInput.focus();
        }
      }, 100);
    }
  }

  getUniqueYears(): number[] {
    const years = [...new Set(this.books.map(book => book.publishedYear))];
    return years.sort((a, b) => b - a);
  }
} 