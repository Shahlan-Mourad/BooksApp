export interface Quote {
  id: number;
  text: string;
  author: string;
  category: string;
  isFavorite?: boolean;
}

export interface CreateQuoteRequest {
  text: string;
  author: string;
  category: string;
}

export interface UpdateQuoteRequest {
  text: string;
  author: string;
  category: string;
} 