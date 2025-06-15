# BookApp Angular 18 Project Structure

## Overview
This project follows a modern Angular 18 architecture with feature-based organization and clear separation of concerns.

## Directory Structure

```
src/app/
├── core/                    # Core application services and utilities
│   ├── services/           # Global services (Auth, Book, etc.)
│   ├── guards/             # Route guards
│   ├── interceptors/       # HTTP interceptors
│   ├── models/             # Shared interfaces and types
│   └── index.ts            # Core module exports
├── shared/                 # Shared components and utilities
│   ├── components/         # Reusable UI components
│   ├── pipes/              # Custom pipes
│   └── index.ts            # Shared module exports
├── features/               # Feature modules
│   ├── books/              # Books feature
│   │   ├── book-list/
│   │   ├── book-detail/
│   │   ├── favorites/
│   │   └── index.ts
│   ├── auth/               # Authentication feature
│   │   ├── login/
│   │   ├── register/
│   │   └── index.ts
│   └── quotes/             # Quotes feature (future)
├── app.component.ts        # Root component
├── app.routes.ts          # Main routing configuration
└── app.config.ts          # Application configuration
```

## Architecture Principles

### Core Module
- **Services**: Global services that are used across multiple features
- **Guards**: Route protection and navigation guards
- **Interceptors**: HTTP request/response interceptors
- **Models**: Shared interfaces, types, and enums

### Shared Module
- **Components**: Reusable UI components (navbar, buttons, cards, etc.)
- **Pipes**: Custom pipes for data transformation
- **Directives**: Custom directives

### Features Module
- **Books**: All book-related functionality
- **Auth**: Authentication and user management
- **Quotes**: Quote management (future feature)

## Benefits of This Structure

1. **Scalability**: Easy to add new features without affecting existing code
2. **Maintainability**: Clear separation of concerns
3. **Reusability**: Shared components can be used across features
4. **Lazy Loading**: Features can be lazy-loaded for better performance
5. **Team Collaboration**: Different teams can work on different features
6. **Testing**: Easier to write and organize tests

## Import Guidelines

- Use relative imports within the same feature
- Use barrel exports (index.ts) for cleaner imports
- Import from `core/` for global services and utilities
- Import from `shared/` for reusable components

## Example Imports

```typescript
// // Import from core
// import { AuthService, BookService } from '../../../core';

// // Import from shared
// import { NavbarComponent } from '../../../shared';

// // Import from features
// import { BookListComponent } from '../book-list/book-list.component';
``` 