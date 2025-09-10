<script lang="ts">
  import '$src/app.css';
  import { logout } from './logout/auth.remote';
  import { goto } from '$app/navigation';

  const handleLogout = async () => {
    try {
      await logout();
      goto('/login');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };
</script>

<div class="min-h-screen bg-gradient-to-br from-blue-50 to-pink-50 flex flex-col">
  <!-- Header / Navbar -->
  <header class="sticky top-0 z-20 bg-white/90 backdrop-blur border-b border-gray-200 shadow-sm rounded-b-2xl mx-4 mt-4">
    <nav class="max-w-4xl mx-auto flex items-center justify-between h-16 px-4 md:px-8">
      <div class="flex items-center space-x-3">
        <img src="/favicon.png" alt="Logo" class="w-8 h-8 rounded-3xl" />
        <span class="text-xl font-bold text-gray-800">Svelte-NET Demo</span>
      </div>

      <div class="flex items-center space-x-6">
        <button
          onclick={() => goto('/pokemon')}
          class="px-4 py-2 bg-gradient-to-r from-indigo-500 to-purple-600 text-white font-medium rounded-3xl transition-colors shadow-sm hover:shadow-md"
        >
          Pokémon
        </button>

        <div class="h-6 w-px bg-gray-300"></div>

        <button
          onclick={handleLogout}
          class="px-4 py-2 bg-red-500 hover:bg-red-600 text-white font-medium rounded-3xl transition-colors shadow-sm hover:shadow-md"
        >
          Logout
        </button>
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
