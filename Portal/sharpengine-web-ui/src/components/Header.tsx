import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ChevronDown, User } from 'lucide-react';

//import { HeaderLogin } from 'sharpengine-ui-shared/src/components/HeaderLogin'

export function Header() {
  const navigate = useNavigate();
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [isLoggedIn] = useState(false); // Change this to true to simulate logged in state

  return (
    <header className="fixed top-0 left-0 right-0 z-50 bg-black/80 backdrop-blur-sm border-b border-white/10">
      <div className="max-w-7xl mx-auto px-6 py-4 flex items-center justify-between">
        {/* Logo */}
        <div className="flex items-center gap-2">
          <div className="w-10 h-10 rounded-lg flex items-center justify-center">
            <img style={{filter: 'invert(1)'}} src="https://raw.githubusercontent.com/Av3boy/SharpEngine/main/Resources/icon.svg" alt="SharpEngine Logo" />
          </div>
          <span className="text-white text-xl">SharpEngine</span>
        </div>

        {/* Navigation */}
        <nav className="flex items-center gap-8">
          <a href="/downloads" className="text-white/80 hover:text-white transition-colors">
            Download
          </a>
          
          {/* Tutorials Dropdown */}
          <div className="relative">
            <button
              onClick={() => setIsDropdownOpen(!isDropdownOpen)}
              className="flex items-center gap-1 text-white/80 hover:text-white transition-colors"
            >
              Tutorials
              <ChevronDown className={`w-4 h-4 transition-transform ${isDropdownOpen ? 'rotate-180' : ''}`} />
            </button>
            
            {isDropdownOpen && (
              <div className="absolute top-full mt-2 left-0 bg-gray-900 border border-white/10 rounded-lg shadow-xl min-w-[200px] overflow-hidden">
                <a href="#getting-started" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  Getting Started
                </a>
                <a href="#basic-concepts" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  Basic Concepts
                </a>
                <a href="#advanced-techniques" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  Advanced Techniques
                </a>
                <a href="#api-reference" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  API Reference
                </a>
              </div>
            )}
          </div>

          <a href="#documentation" className="text-white/80 hover:text-white transition-colors">
            Documentation
          </a>
          
          <a href="#community" className="text-white/80 hover:text-white transition-colors">
            Community
          </a>
        </nav>

        {/* <HeaderLogin onProfileClicked={() => navigate('/profile')} /> */}
      </div>
    </header>
  );
}
