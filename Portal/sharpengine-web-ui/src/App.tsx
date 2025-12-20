import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Header } from './components/Header';

import HomePage from './pages/HomePage';
import DownloadsPage from './pages/downloadsPage/DownloadsPage';
import UserProfilePage from 'sharpengine-ui-shared/pages/userProfile/UserProfilePage';

import RequireAuth from 'sharpengine-ui-shared/auth/RequireAuth';
import { useAuth0, User } from '@auth0/auth0-react';

import './App.tsx.scss';
import ErrorPage from 'sharpengine-ui-shared/pages/ErrorPage';

export default function App() {
  const { isAuthenticated, user } = useAuth0();
  const auth0User = user as User;

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
