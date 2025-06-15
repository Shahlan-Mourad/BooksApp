import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService, Quote } from '../../../core';
import { QuoteCardComponent } from '../../../shared/components/quote-card/quote-card.component';

@Component({
  selector: 'app-quote-list',
  standalone: true,
  imports: [CommonModule, QuoteCardComponent],
  templateUrl: './quote-list.component.html',
  styleUrls: ['./quote-list.component.scss']
})
export class QuoteListComponent implements OnInit {
  quotes: Quote[] = [
    {
      id: 1,
      text: "The only way to do great work is to love what you do.",
      author: "Steve Jobs",
      category: "Motivation",
      isFavorite: false
    },
    {
      id: 2,
      text: "Life is what happens when you're busy making other plans.",
      author: "John Lennon",
      category: "Life",
      isFavorite: false
    },
    {
      id: 3,
      text: "The future belongs to those who believe in the beauty of their dreams.",
      author: "Eleanor Roosevelt",
      category: "Inspiration",
      isFavorite: false
    },
    {
      id: 4,
      text: "Success is not final, failure is not fatal: it is the courage to continue that counts.",
      author: "Winston Churchill",
      category: "Success",
      isFavorite: false
    },
    {
      id: 5,
      text: "The journey of a thousand miles begins with one step.",
      author: "Lao Tzu",
      category: "Wisdom",
      isFavorite: false
    }
  ];

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }
  }

  goToBooks() {
    this.router.navigate(['/books']);
  }
} 