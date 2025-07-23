<script lang="ts">
	import { api, schemas } from '$api';

	// The type for RandomPokemon using Zod schema
	type RandomPokemon = typeof schemas.RandomPokemon._type;
    let randomPokemon: RandomPokemon = {};
    $effect(() => {
        api.getRandomPokemon().then((result) => {
            randomPokemon = Array.isArray(result) ? result[0] : result;
        });
    });
</script>

<h1>Random Pokemon</h1>
<p>This component demonstrates fetching data using Svelte's new async capabilities.</p>

<!-- Use the new top-level await directly in markup -->
<!-- Wrap in svelte:boundary for pending/error handling as required by the current design -->
<svelte:boundary>
	<!-- Display UI while the promise is pending -->
	{#snippet pending()}
		<p><em>Loading...</em></p>
	{/snippet}

	{#await api.getRandomPokemon() then result}

		{@const pokemon = Array.isArray(result) ? result[0] : result}

		<div class="flex flex-col items-center">
			<img
				src={pokemon.image}
				alt={pokemon.name}
				width="200"
				height="200"
				loading="lazy"
			/>
			<h2 class="text-2xl font-bold">{pokemon.name?.toUpperCase()}</h2> 
			<p class="text-lg">{pokemon.type?.toUpperCase()}</p>
		</div>
	{:catch error}
		<!-- The `catch` block runs if the promise rejects -->
		<p style="color: red;">
			Failed to load Pokemon: {error.message || error.toString()}
		</p>
	{/await}
</svelte:boundary>