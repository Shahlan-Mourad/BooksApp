import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService, Book, UpdateBookRequest, CreateBookRequest, AuthService } from '../../../core';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './book-detail.component.html',
  styleUrls: ['./book-detail.component.scss']
})
export class BookDetailComponent implements OnInit {
  book: Book | null = null;
  isLoading = false;
  errorMessage = '';
  isEditing = false;
  isAdding = false;
  isViewing = false;
  editData: UpdateBookRequest = {
    title: '',
    author: '',
    isbn: '',
    publishedYear: 0,
    description: ''
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.determineMode();
    if (this.isAdding) {
      this.initializeAddMode();
    } else if (this.isEditing) {
      this.loadBook();
    } else {
      this.loadBook();
    }
  }

  private determineMode() {
    const url = this.router.url;
    if (url.includes('/add')) {
      this.isAdding = true;
      this.isEditing = true; // Start in edit mode for adding
    } else if (url.includes('/edit')) {
      this.isEditing = true;
    } else {
      this.isViewing = true;
    }
  }

  private initializeAddMode() {
    this.editData = {
      title: '',
      author: '',
      isbn: '',
      publishedYear: new Date().getFullYear(),
      description: ''
    };
  }

  async loadBook() {
    const bookId = this.route.snapshot.paramMap.get('id');
    if (!bookId) {
      this.router.navigate(['/books']);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    try {
      const result = await this.bookService.getBook(parseInt(bookId));
      if (result.success && result.data) {
        this.book = result.data;
        this.editData = {
          title: this.book.title,
          author: this.book.author,
          isbn: this.book.isbn,
          publishedYear: this.book.publishedYear,
          description: this.book.description
        };
        this.cdr.detectChanges();
      } else {
        this.errorMessage = result.message || 'Book not found';
        setTimeout(() => {
          this.router.navigate(['/books']);
        }, 2000);
      }
    } catch (error) {
      this.errorMessage = 'An error occurred while loading the book';
      console.error('Load book error:', error);
    } finally {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  }

  startEditing() {
    this.isEditing = true;
    if (this.book) {
      this.editData = {
        title: this.book.title,
        author: this.book.author,
        isbn: this.book.isbn,
        publishedYear: this.book.publishedYear,
        description: this.book.description
      };
    }
  }

  cancelEditing() {
    if (this.isAdding) {
      // If adding, just go back to books list
      this.router.navigate(['/books']);
    } else {
      // If editing existing book, go back to view mode
      this.isEditing = false;
      if (this.book) {
        this.editData = {
          title: this.book.title,
          author: this.book.author,
          isbn: this.book.isbn,
          publishedYear: this.book.publishedYear,
          description: this.book.description
        };
      }
    }
  }

  async saveChanges() {
    if (!this.editData.title || !this.editData.author) {
      this.errorMessage = 'Title and author are required';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    try {
      if (this.isAdding) {
        // Create new book
        const createRequest: CreateBookRequest = {
          title: this.editData.title,
          author: this.editData.author,
          isbn: this.editData.isbn,
          publishedYear: this.editData.publishedYear,
          description: this.editData.description
        };

        const result = await this.bookService.addBook(createRequest);
        if (result.success) {
          this.router.navigate(['/books']);
        } else {
          this.errorMessage = result.message || 'Failed to create book';
        }
      } else if (this.book) {
        // Update existing book
        const result = await this.bookService.updateBook(this.book.id, this.editData);
        if (result.success && result.data) {
          this.book = { ...this.book, ...result.data };
          this.isEditing = false;
          this.router.navigate(['/books']);
        } else {
          this.errorMessage = result.message || 'Failed to update book';
        }
      }
    } catch (error) {
      this.errorMessage = 'An error occurred while saving the book';
      console.error('Save book error:', error);
    } finally {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  }

  async deleteBook() {
    if (!this.book) return;

    // Show the Bootstrap modal instead of browser confirm
    const modal = document.getElementById('deleteConfirmModal');
    if (modal) {
      const bootstrapModal = new (window as any).bootstrap.Modal(modal);
      bootstrapModal.show();
    }
  }

  async confirmDelete() {
    if (!this.book) return;

    this.isLoading = true;
    this.errorMessage = '';

    try {
      const result = await this.bookService.deleteBook(this.book.id);
      if (result.success) {
        // Hide the modal
        const modal = document.getElementById('deleteConfirmModal');
        if (modal) {
          const bootstrapModal = (window as any).bootstrap.Modal.getInstance(modal);
          bootstrapModal.hide();
        }
        this.router.navigate(['/books']);
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

  goBack() {
    this.router.navigate(['/books']);
  }
} 