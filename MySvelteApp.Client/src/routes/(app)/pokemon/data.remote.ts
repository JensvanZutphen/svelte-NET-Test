import { query } from '$app/server';
import { getRandomPokemon } from '$api/schema/sdk.gen';

export const getRandomPokemonData = query(async () => {
    const response = await getRandomPokemon();
    return response.data; // Return only the serializable data, not the full response object
});