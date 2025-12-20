import { apiRequest, RestClientResult } from "./restClient";
import { Routes } from "./routes";

import { User } from '@auth0/auth0-spa-js';

// Logs out the current user in the BFF (stateless JWT; mostly for symmetry with Auth0 logout).
export async function Logout(token: string) : Promise<RestClientResult<void>> {
  return await apiRequest<void>(`${Routes.Portal}/api/me/logout`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
  });
}

// Logs in the user in the BFF. Response body is currently unused.
export async function Login(user: User, token: string) : Promise<RestClientResult<void>> {
  // Auth0 `sub` is only used here at the edge to log in / create the DB user.
  return await apiRequest<void>(`${Routes.Portal}/api/me/login`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: {
        user: {
          sub: user.sub,
          email: user.email,
          name: user.name
        }
      }
  });
}

/*export async function getCurrentUserDecks(token: string): Promise<RestClientResult<DeckDto[]>> {
  return await apiRequest<DeckDto[]>(`${Routes.ClientBase}/api/users/me/decks`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}*/