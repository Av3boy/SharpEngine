import { Header } from "./components/Header";
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ErrorEventsProvider, useErrorEvents } from 'sharpengine-ui-shared';

import RequireAuth from 'sharpengine-ui-shared/auth/RequireAuth';
import { useAuth0 } from "@auth0/auth0-react";

import UserProfilePage from 'sharpengine-ui-shared/pages/userProfile/UserProfilePage';
import ErrorPage from 'sharpengine-ui-shared/pages/ErrorPage';
import { HomePage } from "./pages/HomePage";

import { useLocation } from "react-router-dom";

import './App.tsx.scss';
import CheckoutPage from "./pages/CheckoutPage";

import Alert from '@mui/material/Alert';

function ErrorPageWrapper() {
  const location = useLocation();
  return <ErrorPage location={location} />;
}

function GlobalErrorAlert() {
  const { latest, clear } = useErrorEvents();
  if (!latest) return null;
  return (
    <Alert
      severity={latest.severity === 'success' ? 'success' : latest.severity}
      onClose={() => clear(latest.id)}
      style={{ position: 'absolute', width: '100vw' }}
    >
      {latest.message}
    </Alert>
  );
}

export default function App() {
  const { isAuthenticated, user } = useAuth0();

  return (
    <ErrorEventsProvider>
      <Router>
        <div className="min-h-screen bg-gray-50">
          <Header />
          <GlobalErrorAlert />

          <Routes>
            <Route element={<RequireAuth isAllowed={isAuthenticated} />}>
              <Route path="/profile" element={<div className="content"><UserProfilePage /></div>} />
            </Route>
            <Route path="/" element={<HomePage />} />
            <Route path="/checkout" element={<div className="content"><CheckoutPage /></div>} />
            <Route path="*" element={<div className="content"><ErrorPageWrapper /></div>} />
          </Routes>
        </div>
      </Router>
    </ErrorEventsProvider>
  );
}
