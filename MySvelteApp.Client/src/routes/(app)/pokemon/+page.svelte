<script lang="ts">
  import { getRandomPokemonApi } from './data.remote';
  import type { RandomPokemon } from '$api/schema/types.gen';
  import * as Card from '$lib/components/ui/card';
  import { Button } from '$lib/components/ui/button';
  import { Badge } from '$lib/components/ui/badge';
  import * as Alert from '$lib/components/ui/alert';
  import RefreshCw from '@lucide/svelte/icons/refresh-cw';
  import Sparkles from '@lucide/svelte/icons/sparkles';
  import AlertTriangle from '@lucide/svelte/icons/alert-triangle';
  import { Label } from '$lib/components/ui/label';
  let pokemonPromise = $state(getRandomPokemonApi());
  let isRefreshing = $state(false);

  async function refreshPokemon() {
    isRefreshing = true;
    try {
      // Use the remote function's refresh method to trigger a new network request
      await pokemonPromise.refresh();
    } catch (error) {
      console.error('Failed to refresh Pokemon:', error);
    } finally {
      isRefreshing = false;
    }
  }
</script>

<div class="container mx-auto px-4 py-8">
  <div class="max-w-2xl mx-auto space-y-6">
    <!-- Header Section -->
    <div class="text-center space-y-2">
      <Label class="text-2xl font-bold">Random Pokémon</Label>
      <Label class="text-lg font-medium">Discover a new Pokémon every time you click refresh!</Label>
    </div>

    <!-- Button Section -->
    <div class="flex justify-center">
      <Button
        onclick={refreshPokemon}
        disabled={isRefreshing}
        variant="default"
        size="lg"
      >
        {#if isRefreshing}
          <RefreshCw class="mr-2 h-4 w-4 animate-spin" />
          Refreshing...
        {:else}
          <Sparkles class="mr-2 h-4 w-4" />
          Get New Pokémon
        {/if}
      </Button>
    </div>

    <!-- Content Section -->
    <div class="flex justify-center">
      <svelte:boundary>
        {#await pokemonPromise}
          <!-- Loading State -->
          <Card.Root class="w-full max-w-sm">
            <Card.Content class="flex flex-col items-center justify-center py-12">
              <RefreshCw class="h-8 w-8 animate-spin mb-4" />
              <Label class="text-lg font-medium">Loading...</Label>
            </Card.Content>
          </Card.Root>
        {:then pokemon}
          <!-- Success State -->
          <Card.Root class="w-full max-w-sm">
            <Card.Content class="flex flex-col items-center space-y-6 py-8">
              <img
                src={pokemon.image}
                alt={pokemon.name}
                class="w-48 h-48 object-contain"
                width="200"
                height="200"
                loading="eager"
                fetchpriority="high"
                decoding="async"
              />
              <div class="text-center space-y-2">
                <h2 class="text-2xl font-bold">
                  {pokemon.name?.toUpperCase()}
                </h2>
                {#if pokemon.type}
                  <Badge variant="secondary">
                    {pokemon.type?.toUpperCase()}
                  </Badge>
                {/if}
              </div>
            </Card.Content>
          </Card.Root>
        {:catch error}
          <!-- Error State -->
          <Card.Root class="w-full max-w-sm">
            <Card.Content class="py-6">
              <Alert.Root variant="destructive">
                <AlertTriangle class="h-4 w-4" />
                <Alert.Title>Oops! Something went wrong</Alert.Title>
                <Alert.Description>
                  Failed to load Pokémon: {error instanceof Error ? error.message : String(error)}
                </Alert.Description>
              </Alert.Root>
            </Card.Content>
          </Card.Root>
        {/await}
      </svelte:boundary>
    </div>
  </div>
</div>
