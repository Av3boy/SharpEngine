import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Header } from './components/Header';

import HomePage from './pages/HomePage';
import DownloadsPage from './pages/DownloadsPage';
import UserProfilePage from './pages/UserProfilePage';

import './App.tsx.scss';

export default function App() {
  return (
    <Router>
      <div className="bg-black min-h-screen">
        <Header />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/downloads" element={<div className="content"><DownloadsPage /></div>} />
          <Route path="/profile" element={<div className="content"><UserProfilePage /></div>} />
        </Routes>
      </div>
    </Router>
  );
}
