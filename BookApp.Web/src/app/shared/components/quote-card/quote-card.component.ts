import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Quote } from '../../../core'; // Import Quote interface from core/models

@Component({
  selector: 'app-quote-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quote-card.component.html',
  styleUrls: ['./quote-card.component.scss']
})
export class QuoteCardComponent {
  @Input() quote!: Quote; // Input property for the quote object
  @Output() deleteQuote = new EventEmitter<number>();
  @Output() viewQuote = new EventEmitter<number>();
  @Output() editQuote = new EventEmitter<number>();
  @Output() toggleFavorite = new EventEmitter<Quote>();

  onDelete(quoteId: number): void {
    this.deleteQuote.emit(quoteId);
  }

  onView(quoteId: number): void {
    this.viewQuote.emit(quoteId);
  }

  onEdit(quoteId: number): void {
    this.editQuote.emit(quoteId);
  }

  onToggleFavorite(quote: Quote): void {
    this.toggleFavorite.emit(quote);
  }
} 