import { Routes } from './routes';
import { apiRequest, RestClientResult } from "./restClient";

import { Asset } from '../types/asset'

export async function fetchAssets(token: string): Promise<RestClientResult<Asset[]>> {
  return await apiRequest<Asset[]>(`${Routes.AssetStore}/api/assets`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

/*import type { DeckDto } from '../dto/DeckDto';

export async function fetchDecks(token: string): Promise<RestClientResult<DeckDto[]>> {
  return await apiRequest<DeckDto[]>(`${Routes.ClientBase}/api/decks`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function getDeckById(deckId: string, token: string): Promise<RestClientResult<DeckDto>> {
  return await apiRequest<DeckDto>(`${Routes.ClientBase}/api/decks/${deckId}`, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function createDeck(payload: Partial<DeckDto>, token: string): Promise<RestClientResult<DeckDto>> {
  return await apiRequest<DeckDto>(`${Routes.ClientBase}/api/decks`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: payload,
  });
}

export async function updateDeck(payload: Partial<DeckDto>, token: string): Promise<RestClientResult<DeckDto>> {
  return await apiRequest<DeckDto>(`${Routes.ClientBase}/api/decks`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: payload,
  });
}

export async function deleteDeck(deckId: string, token: string): Promise<RestClientResult<void>> {
  return await apiRequest<void>(`${Routes.ClientBase}/api/decks/${deckId}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function fetchRandomDeck(): Promise<RestClientResult<DeckDto>> {
  return await apiRequest<DeckDto>(`${Routes.ClientBase}/api/decks/random`, {
    method: "GET",
  });
}*/
