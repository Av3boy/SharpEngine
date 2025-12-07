export interface Asset {
  id: string;
  title: string;
  author: string;
  price: number;
  image: string;
  rating: number;
  reviews: number;
  keywords: string[];
  featured?: boolean;
}
