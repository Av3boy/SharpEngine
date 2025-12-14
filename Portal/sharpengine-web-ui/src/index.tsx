import React from 'react';
import ReactDOM from 'react-dom/client';

import './index.tsx.scss';
import App from './App';

import reportWebVitals from './reportWebVitals';
import { Auth0Provider } from '@auth0/auth0-react';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

// In CRA, only variables prefixed with REACT_APP_ are exposed at build time
const domain: string = process.env.REACT_APP_AUTH0_DOMAIN || "";
const clientId: string = process.env.REACT_APP_AUTH0_CLIENT_ID || "";

root.render(
  <React.StrictMode>
    <Auth0Provider
      domain={domain}
      clientId={clientId}
      authorizationParams={{
        redirect_uri: window.location.origin,
        //audience: audience,
      }}
    >
    <App />
    </Auth0Provider>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
