import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faBook, faUser, faSignOutAlt, faSignInAlt, faUserPlus, faPlus, faUserCircle, faQuoteLeft } from '@fortawesome/free-solid-svg-icons';
import { AuthService, User } from '../../../core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, FontAwesomeModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit, OnDestroy {
  isMenuCollapsed = true;
  isAuthenticated = false;
  currentUser: User | null = null;
  private userSubscription: Subscription | null = null;

  // FontAwesome icons
  faBook = faBook;
  faUser = faUser;
  faSignOutAlt = faSignOutAlt;
  faSignInAlt = faSignInAlt;
  faUserPlus = faUserPlus;
  faPlus = faPlus;
  faUserCircle = faUserCircle;
  faQuoteLeft = faQuoteLeft;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    // Check authentication status immediately
    this.updateAuthStatus();
    
    // Subscribe to user changes
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.isAuthenticated = this.authService.isAuthenticated();
    });
    
    // Additional check after a short delay to handle browser close/reopen
    setTimeout(() => {
      this.updateAuthStatus();
    }, 200);
  }

  ngOnDestroy() {
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }

  private updateAuthStatus() {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.currentUser = this.authService.getCurrentUser();
  }

  getDisplayName(): string {
    if (!this.currentUser) {
      return 'User';
    }

    // If we have both first name and last name, show them
    if (this.currentUser.firstName && this.currentUser.lastName) {
      return `${this.currentUser.firstName} ${this.currentUser.lastName}`;
    }
    
    // If we only have first name
    if (this.currentUser.firstName) {
      return this.currentUser.firstName;
    }
    
    // If we only have last name
    if (this.currentUser.lastName) {
      return this.currentUser.lastName;
    }
    
    // Fallback to username
    return this.currentUser.username || 'User';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  toggleMenu() {
    this.isMenuCollapsed = !this.isMenuCollapsed;
  }
} 