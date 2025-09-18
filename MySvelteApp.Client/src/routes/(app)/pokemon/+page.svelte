<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '$lib/components/ui/card';
	import { Badge } from '$lib/components/ui/badge';
	import { Skeleton } from '$lib/components/ui/skeleton';
	import { Avatar, AvatarImage, AvatarFallback } from '$lib/components/ui/avatar';
	import { getRandomPokemonData } from './data.remote';
	import SparklesIcon from '@lucide/svelte/icons/sparkles';
	import RefreshCwIcon from '@lucide/svelte/icons/refresh-cw';
	import ZapIcon from '@lucide/svelte/icons/zap';
	import SkullIcon from '@lucide/svelte/icons/skull';
	import { cn } from '$lib/utils';
	import { fade, slide, fly, scale } from 'svelte/transition';

	// Button fade state
	let isButtonFading = $state(false);
	let currentPromise = $state<Promise<unknown> | null>(null);

	// Reset button fade state when the current promise settles
	$effect(() => {
		if (currentPromise) {
			currentPromise
				.finally(() => {
					// Reset after a short delay to allow for smooth transition
					setTimeout(() => {
						isButtonFading = false;
						currentPromise = null;
					}, 300);
				})
				.catch(() => {
					// Handle error case - still reset the button
					setTimeout(() => {
						isButtonFading = false;
						currentPromise = null;
					}, 300);
				});
		}
	});


</script>

<div class="min-h-screen">
	<div class="container mx-auto px-4 py-12 max-w-4xl">
		<!-- Header Section -->
		<div class="text-center mb-8" in:fade>
			<div class="inline-flex items-center gap-2 mb-4" in:fly={{ y: 20 }}>
				<SparklesIcon class="w-8 h-8 text-yellow-500" />
				<h1 class="text-4xl font-bold">
					Pokemon Explorer
				</h1>
				<SparklesIcon class="w-8 h-8 text-yellow-500" />
			</div>
			<p class="text-lg text-muted-foreground mb-6" in:slide>
				Discover amazing Pokemon with a single click! ⚡
			</p>
		</div>

		<!-- Action Button -->
		<div class="flex justify-center mb-8" in:fade>
			<div transition:fade={{ duration: 300 }}>
				<Button
					onclick={() => {
						isButtonFading = true;
						currentPromise = getRandomPokemonData().refresh();
					}}
					size="lg"
					class={cn(
						"font-semibold px-8 py-4 text-lg",
						"shadow-lg hover:shadow-xl transform hover:scale-105",
						isButtonFading && "cursor-not-allowed opacity-50"
					)}
					disabled={isButtonFading}
				>
					<SparklesIcon class="w-5 h-5 mr-2" />
					{isButtonFading ? "Discovering..." : "Discover Pokemon"}
				</Button>
			</div>
		</div>

		<!-- Pokemon Display -->
		<div class="flex justify-center mb-12" in:fade>
			<svelte:boundary>
				{#await getRandomPokemonData() then pokemon}
					<div class="flex flex-col items-center">
						<Card class="w-full max-w-md shadow-2xl border-0 bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm">
						<CardHeader class="text-center pb-4">
							<div class="flex justify-center mb-4">
								<div in:scale={{ delay: 200 }}>
									<Avatar class="w-32 h-32 border-4 border-white shadow-lg">
										<AvatarImage
											src={pokemon?.image || ''}
											alt={pokemon?.name || 'Pokemon'}
											class="object-cover"
										/>
										<AvatarFallback class="text-2xl font-bold bg-gradient-to-br from-blue-400 to-purple-500 text-white">
											{pokemon?.name?.charAt(0)?.toUpperCase() || '?'}
										</AvatarFallback>
									</Avatar>
								</div>
							</div>
							<div in:slide={{ delay: 400 }}>
								<CardTitle class="text-3xl font-bold text-center">
									{pokemon?.name || 'Unknown Pokemon'}
								</CardTitle>
							</div>
						</CardHeader>

						<CardContent class="text-center pb-4">
							{#if pokemon?.type}
								<div class="flex justify-center">
									<div in:fly={{ y: 10, delay: 600 }}>
										<Badge
											variant="default"
											class="px-4 py-2 text-sm font-semibold flex items-center gap-2"
										>
											<ZapIcon class="w-4 h-4" />
											{pokemon.type}
										</Badge>
									</div>
								</div>
							{:else}
								<div class="flex justify-center">
									<div in:fade={{ delay: 600 }}>
										<Badge
											variant="outline"
											class="px-4 py-2 text-sm font-semibold"
										>
											<SparklesIcon class="w-4 h-4 mr-1" />
											Mystery Type
										</Badge>
									</div>
								</div>
							{/if}
						</CardContent>

						<CardFooter class="text-center pt-4 border-t border-gray-100 dark:border-gray-700">
							<div in:fade={{ delay: 800 }}>
								<div class="text-sm text-muted-foreground">
									✨ Caught a wild {pokemon?.name || 'Pokemon'}! ✨
								</div>
							</div>
						</CardFooter>
					</Card>
					</div>
				{/await}

				{#snippet pending()}
					<div class="flex flex-col items-center">
						<Card class="w-full max-w-md mx-auto shadow-2xl border-0 bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm">
						<CardHeader class="text-center pb-4">
							<div class="flex justify-center mb-4">
								<div in:scale>
									<Skeleton class="w-32 h-32 rounded-full" />
								</div>
							</div>
							<div in:slide>
								<Skeleton class="h-8 w-48 mx-auto" />
							</div>
						</CardHeader>

						<CardContent class="text-center pb-4">
							<div in:fade>
								<Skeleton class="h-8 w-24 mx-auto" />
							</div>
						</CardContent>

						<CardFooter class="text-center pt-4 border-t border-gray-100 dark:border-gray-700">
							<div in:fade>
								<Skeleton class="h-4 w-40 mx-auto" />
							</div>
						</CardFooter>
					</Card>

					<!-- Fun Stats Section - Loading State -->
					<div class="mt-8" in:fade={{ delay: 1000 }}>
						<!-- Add a subtle loading hint -->
						<div class="mt-4 text-center text-sm text-muted-foreground opacity-70" in:fade={{ delay: 1600 }}>
							🔍 Searching for Pokemon...
						</div>
					</div>
					</div>
				{/snippet}

				{#snippet onerror(err: unknown)}
					<div class="flex flex-col items-center">
						<Card class="w-full max-w-md mx-auto shadow-2xl border-0 bg-red-50 dark:bg-red-900/20">
						<CardContent class="text-center py-8">
							<div class="text-red-500 mb-4">
								<div in:scale>
									<SkullIcon class="w-12 h-12 mx-auto" />
								</div>
							</div>
							<div in:fade>
								<h3 class="text-lg font-semibold text-red-800 dark:text-red-200 mb-2">
									Oops! Pokemon escaped!
								</h3>
							</div>
							<div in:slide>
								<p class="text-sm text-red-600 dark:text-red-300 mb-4">
									{err instanceof Error ? err.message : 'Something went wrong while catching Pokemon'}
								</p>
							</div>
							<Button
								onclick={() => getRandomPokemonData().refresh()}
								variant="outline"
								class="border-red-300 text-red-700 hover:bg-red-50"
							>
								<RefreshCwIcon class="w-4 h-4 mr-2" />
								Try Again
							</Button>
						</CardContent>
					</Card>
					</div>
				{/snippet}
			</svelte:boundary>
		</div>
	</div>
</div>

