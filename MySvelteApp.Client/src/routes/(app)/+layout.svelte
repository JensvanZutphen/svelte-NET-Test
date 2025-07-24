<script>
  import '$src/app.css';
</script>

<div class="min-h-screen bg-gradient-to-br from-blue-50 to-pink-50 flex flex-col">
  <!-- Header / Navbar -->
  <header class="sticky top-0 z-20 bg-white/80 backdrop-blur border-b border-gray-200 shadow-sm">
    <nav class="max-w-4xl mx-auto flex items-center justify-between h-16 px-4 md:px-8">
      <div class="flex items-center space-x-3">
        <img src="/favicon.png" alt="Logo" class="w-8 h-8" />
        <span class="text-xl font-bold text-pink-700 tracking-wide">Svelte-NET Demo</span>
      </div>
      <div class="flex items-center space-x-6">
        <a href="/" class="text-blue-600 hover:text-pink-600 font-medium transition-colors">Home</a>
        <a href="/fetchdata" class="text-blue-600 hover:text-pink-600 font-medium transition-colors">Fetch Data</a>
        <a
          href="https://docs.microsoft.com/aspnet/"
          target="_blank"
          rel="noreferrer"
          class="text-blue-600 hover:text-pink-600 font-medium transition-colors"
        >About</a>
        <form method="POST" action="/logout">
          <button 
            type="submit"
            class="text-blue-600 hover:text-pink-600 font-medium transition-colors"
          >
            Logout
          </button>
        </form>
      </div>
    </nav>
  </header>

  <!-- Main Content -->
  <main class="flex-1 flex flex-col items-center justify-center py-8">
    <article class="w-full max-w-2xl bg-white/90 rounded-3xl shadow-xl p-6 md:p-10 mt-6">
      <slot />
    </article>
  </main>
</div>
