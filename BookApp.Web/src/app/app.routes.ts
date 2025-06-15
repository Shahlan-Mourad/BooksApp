import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, title: 'Log in' },
  { path: 'register', component: RegisterComponent, title: 'Register' },
  {
    path: 'books',
    canActivate: [AuthGuard],
    children: [
      { path: '', loadComponent: () => import('./features/books/book-list/book-list.component').then(m => m.BookListComponent), title: 'Books' },
      { path: 'add', loadComponent: () => import('./features/books/book-detail/book-detail.component').then(m => m.BookDetailComponent), title: 'Add Book' },
      { path: 'edit/:id', loadComponent: () => import('./features/books/book-detail/book-detail.component').then(m => m.BookDetailComponent), title: 'Edit Book' },
      { path: ':id', loadComponent: () => import('./features/books/book-detail/book-detail.component').then(m => m.BookDetailComponent), title: 'Book Details' }
    ]
  },
  {
    path: 'quotes',
    canActivate: [AuthGuard],
    loadComponent: () => import('./features/quotes/quote-list/quote-list.component').then(m => m.QuoteListComponent),
    title: 'My Quotes'
  },
  {
    path: 'profile',
    canActivate: [AuthGuard],
    loadComponent: () => import('./features/user/profile/profile.component').then(m => m.ProfileComponent),
    title: 'My Profile'
  },
  { path: '**', redirectTo: '/login' }
];