import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Header } from './components/Header';

import HomePage from './pages/HomePage';
import DownloadsPage from './pages/DownloadsPage';
import UserProfilePage from './pages/UserProfilePage';

export default function App() {
  return (
    <Router>
      <div className="bg-black min-h-screen">
        <Header />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/downloads" element={<DownloadsPage />} />
          <Route path="/profile" element={<UserProfilePage />} />
        </Routes>
      </div>
    </Router>
  );
}
