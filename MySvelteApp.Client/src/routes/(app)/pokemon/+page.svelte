<script lang="ts">
  import { getRandomPokemon } from '$api/schema/sdk.gen';
  import type { RandomPokemon } from '$api/schema/types.gen';
  import { onMount } from 'svelte';

  let pokemonPromise = $state<Promise<any> | null>(null);

  onMount(() => {
    pokemonPromise = getRandomPokemon();
  });

  function refreshPokemon() {
    pokemonPromise = getRandomPokemon();
  }
</script>

<button
  class="mb-8 px-6 py-2 rounded-full bg-pink-500 hover:bg-pink-600 text-white font-semibold shadow transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
  onclick={refreshPokemon}
>
  🔄 Refresh
</button>

<svelte:boundary>
  <div class="flex flex-col items-center justify-center min-h-[60vh]">
    <h1 class="text-3xl md:text-4xl font-extrabold mb-2 text-pink-700 drop-shadow">Random Pokémon</h1>
    <p class="mb-6 text-lg text-gray-700">Discover a new Pokémon every time you click refresh!</p>

    <div>
      {#if pokemonPromise}
        {#await pokemonPromise}
          <div class="flex items-center justify-center h-64 w-80 bg-white bg-opacity-80 rounded-2xl shadow-lg animate-pulse">
            <p class="text-xl text-gray-500">Loading...</p>
          </div>
        {:then response}
          <div class="w-80 bg-white bg-opacity-90 rounded-3xl shadow-2xl flex flex-col items-center p-8 transition-all">
            <img
              src={response.data?.image}
              alt={response.data?.name}
              class="w-40 h-40 object-contain mb-4 drop-shadow-lg rounded-full border-4 border-pink-200 bg-pink-50"
              width="200"
              height="200"
              loading="lazy"
            />
            <h2 class="text-2xl font-bold text-blue-700 mb-2 tracking-wide">{response.data?.name?.toUpperCase()}</h2>
            <p class="text-lg font-medium text-pink-600 mb-2">{response.data?.type?.toUpperCase()}</p>
          </div>
        {:catch error}
          <div class="flex items-center justify-center h-64 w-80 bg-white bg-opacity-80 rounded-2xl shadow-lg">
            <p class="text-red-600 font-semibold text-center">
              Failed to load Pokémon: {error instanceof Error ? error.message : String(error)}
            </p>
          </div>
        {/await}
      {:else}
        <div class="flex items-center justify-center h-64 w-80 bg-white bg-opacity-80 rounded-2xl shadow-lg">
          <p class="text-xl text-gray-500">Loading...</p>
        </div>
      {/if}
    </div>
  </div>
</svelte:boundary>
