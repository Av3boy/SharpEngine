import { AssetCard } from "../components/AssetCard";
import { Asset } from "../types";

const mockAssets: Asset[] = [
  {
    id: "1",
    title: "Realistic Character Model Pack",
    author: "DigitalStudios",
    price: 49.99,
    image: "https://images.unsplash.com/photo-1650081221669-fccab00c76a0?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHwzZCUyMGNoYXJhY3RlciUyMG1vZGVsfGVufDF8fHx8MTc2NTEyODEzM3ww&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.8,
    reviews: 234,
    keywords: ["3d", "character", "model", "realistic"],
    featured: true,
  },
  {
    id: "2",
    title: "Fantasy Environment Textures",
    author: "TextureMasters",
    price: 29.99,
    image: "https://images.unsplash.com/photo-1628523413845-7c235af63fa4?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxnYW1lJTIwZW52aXJvbm1lbnQlMjB0ZXh0dXJlfGVufDF8fHx8MTc2NTEyODEzM3ww&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.6,
    reviews: 189,
    keywords: ["texture", "environment", "fantasy", "pbr"],
  },
  {
    id: "3",
    title: "Epic Landscape Collection",
    author: "WorldBuilders",
    price: 79.99,
    image: "https://images.unsplash.com/photo-1534447677768-be436bb09401?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxmYW50YXN5JTIwbGFuZHNjYXBlfGVufDF8fHx8MTc2NTExMjE1NHww&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.9,
    reviews: 412,
    keywords: ["landscape", "environment", "terrain", "nature"],
    featured: true,
  },
  {
    id: "4",
    title: "Pixel Art Asset Bundle",
    author: "PixelCraft",
    price: 19.99,
    image: "https://images.unsplash.com/photo-1668119065849-d8a7e9d73a7b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxwaXhlbCUyMGFydCUyMGdhbWV8ZW58MXx8fHwxNzY1MDI2NzA4fDA&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.7,
    reviews: 567,
    keywords: ["pixel art", "2d", "sprites", "retro"],
  },
  {
    id: "5",
    title: "Modern UI Kit Pro",
    author: "UIDesigns",
    price: 34.99,
    image: "https://images.unsplash.com/photo-1586763749650-70d7996310d0?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxnYW1lJTIwdWklMjBpbnRlcmZhY2V8ZW58MXx8fHwxNzY1MTI4MTM0fDA&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.5,
    reviews: 298,
    keywords: ["ui", "interface", "hud", "menu"],
  },
  {
    id: "6",
    title: "Low Poly Vehicle Pack",
    author: "VehiclePro",
    price: 39.99,
    image: "https://images.unsplash.com/photo-1643299420057-3ebf4d5af17d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHwzZCUyMG1vZGVsJTIwY2FyfGVufDF8fHx8MTc2NTEyODEzNHww&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.8,
    reviews: 345,
    keywords: ["vehicle", "car", "3d", "low poly"],
    featured: true,
  },
  {
    id: "7",
    title: "Medieval Castle Bundle",
    author: "CastleWorks",
    price: 59.99,
    image: "https://images.unsplash.com/photo-1485465053475-dd55ed3894b9?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxtZWRpZXZhbCUyMGNhc3RsZXxlbnwxfHx8fDE3NjUxMjgxMzV8MA&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.9,
    reviews: 423,
    keywords: ["medieval", "castle", "building", "architecture"],
  },
  {
    id: "8",
    title: "Weapon Arsenal Collection",
    author: "ArmsDealer",
    price: 44.99,
    image: "https://images.unsplash.com/photo-1730578725185-3810188ecf8c?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxnYW1lJTIwd2VhcG9uc3xlbnwxfHx8fDE3NjUxMjgxMzV8MA&ixlib=rb-4.1.0&q=80&w=1080",
    rating: 4.7,
    reviews: 387,
    keywords: ["weapon", "sword", "gun", "equipment"],
  },
];

export function HomePage() {
    return (
        <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            {/* Hero Section */}
            <div className="mb-8">
              <h1 className="text-gray-900 mb-2">Popular Assets</h1>
              <p className="text-gray-600">
                Discover high-quality assets for your next game project
              </p>
            </div>

            {/* Assets Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {mockAssets.map((asset) => (
                <AssetCard key={asset.id} asset={asset} />
              ))}
            </div>
        </main>
    );
}
