import { Star } from "lucide-react";
import { Asset } from "../types";

interface AssetCardProps {
  asset: Asset;
}

export function AssetCard({ asset }: AssetCardProps) {
  return (
    <div className="bg-white rounded-lg overflow-hidden shadow-md hover:shadow-xl transition-shadow cursor-pointer group">
      {/* Image */}
      <div className="relative aspect-video overflow-hidden bg-gray-100">
        <img
          src={asset.image}
          alt={asset.title}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
        />
        {asset.featured && (
          <div className="absolute top-2 right-2 bg-yellow-400 text-gray-900 px-2 py-1 rounded text-xs">
            Featured
          </div>
        )}
      </div>
      
      {/* Content */}
      <div className="p-4">
        <h3 className="text-gray-900 mb-1 truncate">{asset.title}</h3>
        <p className="text-sm text-gray-500 mb-3">by {asset.author}</p>
        
        {/* Keywords */}
        <div className="flex flex-wrap gap-1 mb-3">
          {asset.keywords.slice(0, 3).map((keyword) => (
            <span
              key={keyword}
              className="text-xs bg-gray-100 text-gray-600 px-2 py-1 rounded"
            >
              {keyword}
            </span>
          ))}
        </div>
        
        {/* Footer */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-1">
            <Star className="w-4 h-4 text-yellow-400 fill-yellow-400" />
            <span className="text-sm text-gray-700">{asset.rating.toFixed(1)}</span>
            <span className="text-sm text-gray-400">({asset.reviews})</span>
          </div>
          <span className="text-indigo-600">${asset.price.toFixed(2)}</span>
        </div>
      </div>
    </div>
  );
}
