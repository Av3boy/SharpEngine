import { Asset } from "../types";

interface SearchDropdownProps {
  assets: Asset[];
  onClose: () => void;
}

export function SearchDropdown({ assets, onClose }: SearchDropdownProps) {
  if (assets.length === 0) {
    return (
      <div className="absolute top-full mt-2 w-full bg-white border border-gray-200 rounded-lg shadow-lg p-4">
        <p className="text-gray-500 text-center">No assets found</p>
      </div>
    );
  }

  return (
    <div className="absolute top-full mt-2 w-full bg-white border border-gray-200 rounded-lg shadow-lg max-h-96 overflow-y-auto">
      {assets.map((asset) => (
        <div
          key={asset.id}
          className="flex items-center gap-4 p-3 hover:bg-gray-50 cursor-pointer border-b border-gray-100 last:border-b-0"
          onClick={onClose}
        >
          {/* Preview Image */}
          <img
            src={asset.image}
            alt={asset.title}
            className="w-16 h-16 object-cover rounded"
          />
          
          {/* Asset Info */}
          <div className="flex-1 min-w-0">
            <h3 className="text-gray-900 truncate">{asset.title}</h3>
            <p className="text-sm text-gray-500">by {asset.author}</p>
          </div>
          
          {/* Price */}
          <div className="text-indigo-600">
            ${asset.price.toFixed(2)}
          </div>
        </div>
      ))}
    </div>
  );
}
