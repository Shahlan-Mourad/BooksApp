export interface Book {
  id: number;
  title: string;
  author: string;
  isbn: string;
  publishedYear: number;
  description: string;
  coverImageUrl?: string;
  isFavorite?: boolean;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  isbn: string;
  publishedYear: number;
  description: string;
}

export interface UpdateBookRequest {
  title: string;
  author: string;
  isbn: string;
  publishedYear: number;
  description: string;
} 