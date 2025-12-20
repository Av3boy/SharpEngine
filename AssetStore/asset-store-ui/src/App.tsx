import { Header } from "./components/Header";
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

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

export default function App() {
    const { isAuthenticated, user } = useAuth0();

  return (
    <Router>
      <div className="min-h-screen bg-gray-50">
        <Header />
        <Alert severity="success" onClose={() => {}} style={{position: 'absolute', width: '100vw'}}>
          This is a success Alert.
        </Alert>
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
  );
}
