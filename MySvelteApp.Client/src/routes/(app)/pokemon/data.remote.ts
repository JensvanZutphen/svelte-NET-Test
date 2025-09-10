import { query } from '$app/server';
import type { RandomPokemon as RandomPokemonType } from '$api/schema/types.gen';
import { getRandomPokemon } from '$api/schema/sdk.gen';

// Remote function for server-side data loading
export const getRandomPokemonApi = query(async (): Promise<RandomPokemonType> => {
    const response = await getRandomPokemon();
    if (response.data) {
        return response.data;
    }
    throw new Error('Failed to fetch random Pokemon');
});