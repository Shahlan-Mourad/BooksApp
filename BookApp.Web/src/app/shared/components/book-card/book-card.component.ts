import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Book } from '../../../core'; // Import Book interface from core/models

@Component({
  selector: 'app-book-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './book-card.component.html',
  styleUrls: ['./book-card.component.scss']
})
export class BookCardComponent {
  @Input() book!: Book; // Input property for the book object
  @Output() deleteBook = new EventEmitter<number>();
  @Output() viewBook = new EventEmitter<number>();
  @Output() editBook = new EventEmitter<number>();

  onDelete(bookId: number): void {
    this.deleteBook.emit(bookId);
  }

  onView(bookId: number): void {
    this.viewBook.emit(bookId);
  }

  onEdit(bookId: number): void {
    this.editBook.emit(bookId);
  }
} 