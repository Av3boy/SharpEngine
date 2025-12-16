import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Header } from './components/Header';

import HomePage from './pages/HomePage';
import DownloadsPage from './pages/downloadsPage/DownloadsPage';
import UserProfilePage from './pages/userProfile/UserProfilePage';

import RequireAuth from './auth/RequireAuth';
import { useAuth0 } from '@auth0/auth0-react';

import './App.tsx.scss';
import ErrorPage from './pages/ErrorPage';

export default function App() {
    const { isAuthenticated, user } = useAuth0();

  return (
    <Router>
      <div className="min-h-screen">
        <Header />
        <Routes>
          <Route element={<RequireAuth isAllowed={isAuthenticated} />}>
            <Route path="/profile" element={<div className="content"><UserProfilePage /></div>} />
          </Route>
          <Route path="/" element={<HomePage />} />
          <Route path="/downloads" element={<div className="content"><DownloadsPage /></div>} />
          <Route path="*" element={<div className="content"><ErrorPage /></div>} />
        </Routes>
      </div>
    </Router>
  );
}
