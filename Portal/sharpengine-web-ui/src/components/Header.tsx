import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { HeaderLogin } from 'sharpengine-ui-shared'
import { useAuth0 } from '@auth0/auth0-react';
import './Header.scss';

export function Header() {
  const navigate = useNavigate();
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const { loginWithRedirect, logout, isAuthenticated, user } = useAuth0();

  return (
    <header className="header fixed top-0 left-0 right-0 z-50 bg-black/80 backdrop-blur-sm border-b border-white/10">
      <div className="max-w-7xl mx-auto px-6 py-4 flex items-center justify-between">
        {/* Logo */}
        <div className="flex items-center gap-2" onClick={() => navigate("/")} style={{cursor: "pointer"}}>
          <div className="w-10 h-10 rounded-lg flex items-center justify-center">
            <img style={{filter: 'invert(1)'}} src="icon.svg" alt="SharpEngine Logo" />
          </div>
          <span className="text-white text-xl">SharpEngine</span>
        </div>

        {/* Desktop Navigation */}
        <nav className="header__nav flex items-center gap-8">
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
              {/*<ChevronDown className={`w-4 h-4 transition-transform ${isDropdownOpen ? 'rotate-180' : ''}`} />*/}
            </button>
            
            {isDropdownOpen && (
              <div className="absolute top-full mt-2 left-0 bg-gray-900 border border-white/10 rounded-lg shadow-xl min-w-[200px] overflow-hidden">
                <a href="/docs#getting-started" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  Getting Started
                </a>
                <a href="/docs#basic-concepts" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  Basic Concepts
                </a>
                <a href="/docs#advanced-techniques" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  Advanced Techniques
                </a>
                <a href="/docs#api-reference" className="block px-4 py-3 text-white/80 hover:bg-white/5 hover:text-white transition-colors">
                  API Reference
                </a>
              </div>
            )}
          </div>

          <a href="/docs" className="text-white/80 hover:text-white transition-colors">
            Documentation
          </a>
          
          <a href="#community" className="text-white/80 hover:text-white transition-colors">
            Community
          </a>
        </nav>

        {/* Burger button (mobile) */}
        <button
          className="header__burger text-white/90 hover:text-white"
          aria-label="Toggle menu"
          aria-expanded={isMobileMenuOpen}
          onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
        >
          <span className={`burger-lines ${isMobileMenuOpen ? 'open' : ''}`}></span>
        </button>

        <div className="header__login-desktop">
          <HeaderLogin onProfileClicked={() => navigate('/profile')} 
                       loginWithRedirect={loginWithRedirect} 
                       logout={logout} 
                       isAuthenticated={isAuthenticated} 
                       user={user}
                       useWhiteText={true} />
        </div>
      </div>

      {/* Mobile menu panel */}
      {isMobileMenuOpen && (
        <div className="header__mobile-panel bg-black/90 border-t border-white/10">
          <nav className="flex flex-col gap-2 px-6 py-4">
            <a href="/downloads" className="text-white/80 hover:text-white transition-colors py-2">Download</a>
            
            <details className="mobile-details">
              <summary className="text-white/80 hover:text-white transition-colors py-2">Tutorials</summary>
              <div className="flex flex-col pl-4">
                <a href="/docs#getting-started" className="text-white/70 hover:text-white py-1">Getting Started</a>
                <a href="/docs#basic-concepts" className="text-white/70 hover:text-white py-1">Basic Concepts</a>
                <a href="/docs#advanced-techniques" className="text-white/70 hover:text-white py-1">Advanced Techniques</a>
                <a href="/docs#api-reference" className="text-white/70 hover:text-white py-1">API Reference</a>
              </div>
            </details>

            <a href="/docs" className="text-white/80 hover:text-white transition-colors py-2">Documentation</a>
            <a href="#community" className="text-white/80 hover:text-white transition-colors py-2">Community</a>
            
            <HeaderLogin onProfileClicked={() => navigate('/profile')} 
                         loginWithRedirect={loginWithRedirect} 
                         logout={logout} 
                         isAuthenticated={isAuthenticated} 
                         user={user}
                         useWhiteText={true} />
          </nav>
        </div>
      )}
    </header>
  );
}
