<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { getRandomPokemonData } from './data.remote';
	import { Card, CardContent } from '$lib/components/ui/card';
</script>

<div class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8">
	<Button onclick={() => getRandomPokemonData()}>Click me to see a random pokemon</Button>
	<div class="flex flex-col gap-4">

	<svelte:boundary>
		{#await getRandomPokemonData() then pokemon}
		<Card>	
			<CardContent>
				<div>Pokemon: {pokemon?.name}</div>
				<img src={pokemon?.image} alt={pokemon?.name} />
				<div>Type: {pokemon?.type}</div>
			</CardContent>
		</Card>
		{/await}

		{#snippet pending()}
			<div>Loading...</div>
		{/snippet}
	</svelte:boundary>
	</div>
</div>