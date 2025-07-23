<script>
	import '../app.css';
</script>

<div class="flex min-h-screen flex-col md:flex-row">
	<main class="flex-1">
		<div
			class="sticky top-0 z-10 flex h-14 items-center justify-end border-b border-gray-300 bg-gray-100 px-4 md:justify-end md:px-8"
		>
			<a href="/" class="ml-0 truncate text-blue-600 hover:underline md:w-0 md:flex-1 md:text-right"
				>Home</a
			>
			<a href="/fetchdata" class="ml-6 text-blue-600 hover:underline">Fetch data</a>
			<a
				href="https://docs.microsoft.com/aspnet/"
				target="_blank"
				rel="noreferrer"
				class="ml-6 text-blue-600 hover:underline">About</a
			>
		</div>
		<article class="content px-4 md:px-8">
			<slot />
		</article>
	</main>
</div>
