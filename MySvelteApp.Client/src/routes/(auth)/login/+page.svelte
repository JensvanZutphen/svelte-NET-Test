<script lang="ts">
  import { enhance } from '$app/forms';
  import { goto } from '$app/navigation';

  interface FormProps {
    error?: string;
    username?: string;
  }

  export let form: FormProps | null = null;

  // Preserve form values after failed submission
  let username = form?.username || '';

  // Handle form submission result
  const handleSubmit = () => {
    return async ({ result }: { result: any }) => {
      if (result.type === 'success' && result.data?.success) {
        console.log('Login successful, navigating to /pokemon');
        // Wait a bit to ensure the cookie is set
        await new Promise(resolve => setTimeout(resolve, 100));
        // Navigate to the Pokemon page after successful login
        await goto('/pokemon');
      }
    };
  };
</script>

<div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full space-y-8">
    <div>
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Sign in to your account
      </h2>
      <p class="mt-2 text-center text-sm text-gray-600">
        Don't have an account?
        <a href="/register" class="font-medium text-indigo-600 hover:text-indigo-500">
          Sign up
        </a>
      </p>
    </div>
    
    <form method="POST" use:enhance={handleSubmit} class="mt-8 space-y-6">
      <div class="rounded-md shadow-sm -space-y-px">
        <input
          type="text"
          name="username"
          placeholder="Username"
          bind:value={username}
          class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
        />
        <input
          type="password"
          name="password"
          placeholder="Password"
          class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
        />
      </div>

      {#if form?.error}
        <div class="rounded-md bg-red-50 p-4">
          <div class="text-sm text-red-700">{form.error}</div>
        </div>
      {/if}

      <button
        type="submit"
        class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
      >
        Sign in
      </button>
    </form>
  </div>
</div>
