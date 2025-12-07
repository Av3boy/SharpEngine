import { Search, ShoppingCart, User } from "lucide-react";
import { useState, useEffect, useRef } from "react";
import { SearchDropdown } from "./SearchDropdown";
import { Asset } from "../types";

interface HeaderProps {
  assets: Asset[];
}

export function Header({ assets }: HeaderProps) {
  const [searchQuery, setSearchQuery] = useState("");
  const [showDropdown, setShowDropdown] = useState(false);
  const searchRef = useRef<HTMLDivElement>(null);

  const filteredAssets = searchQuery.trim()
    ? assets.filter((asset) => {
        const query = searchQuery.toLowerCase();
        return (
          asset.title.toLowerCase().includes(query) ||
          asset.keywords.some((keyword) => keyword.toLowerCase().includes(query)) ||
          asset.author.toLowerCase().includes(query)
        );
      })
    : [];

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (searchRef.current && !searchRef.current.contains(event.target as Node)) {
        setShowDropdown(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <header className="bg-white border-b border-gray-200 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <div className="flex items-center">
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center">
                <ShoppingCart className="w-5 h-5 text-white" />
              </div>
              <span className="text-xl text-gray-900">AssetStore</span>
            </div>
          </div>

          {/* Search Bar */}
          <div ref={searchRef} className="flex-1 max-w-2xl mx-8 relative">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                type="text"
                placeholder="Search assets by name, keyword, or author..."
                value={searchQuery}
                onChange={(e) => {
                  setSearchQuery(e.target.value);
                  setShowDropdown(true);
                }}
                onFocus={() => setShowDropdown(true)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-600 focus:border-transparent"
              />
            </div>
            
            {showDropdown && searchQuery.trim() && (
              <SearchDropdown 
                assets={filteredAssets} 
                onClose={() => setShowDropdown(false)}
              />
            )}
          </div>

          {/* Navigation */}
          <div className="flex items-center gap-4">
            <button className="px-4 py-2 text-gray-700 hover:text-gray-900 transition-colors">
              Sign In
            </button>
            <button className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors flex items-center gap-2">
              <User className="w-4 h-4" />
              Sign Up
            </button>
          </div>
        </div>
      </div>
    </header>
  );
}
