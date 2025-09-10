<script lang="ts">
  import { login } from './auth.remote';
  import { goto } from '$app/navigation';

  // Form state with runes
  let username = $state('');
  let password = $state('');
  let isLoading = $state(false);
  let fieldErrors = $state({
    username: '',
    password: ''
  });

  // Validate fields
  const validateField = (field: string, value: string) => {
    switch (field) {
      case 'username':
        if (!value.trim()) {
          fieldErrors.username = 'Username is required';
        } else if (value.length < 3) {
          fieldErrors.username = 'Username must be at least 3 characters';
        } else {
          fieldErrors.username = '';
        }
        break;
      case 'password':
        if (!value) {
          fieldErrors.password = 'Password is required';
        } else if (value.length < 6) {
          fieldErrors.password = 'Password must be at least 6 characters';
        } else {
          fieldErrors.password = '';
        }
        break;
    }
  };

  // Validate form before submission
  const validateForm = () => {
    // Validate all fields
    validateField('username', username);
    validateField('password', password);

    // Check if there are any field errors
    return !Object.values(fieldErrors).some(error => error !== '');
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
        <a href="/register" class="font-medium text-indigo-600 hover:text-indigo-500 transition-colors">
          Sign up
        </a>
      </p>
    </div>

    <form {...login.enhance(async ({ form, submit }) => {
      // Validate form before submission
      if (!validateForm()) {
        return; // Don't submit if validation fails
      }

      isLoading = true;
      try {
        await submit();
        // Check result after submission
        if (login.result?.success) {
          console.log('Login successful, navigating to /pokemon');
          await goto('/pokemon');
        }
      } catch (error) {
        console.error('Login error:', error);
      } finally {
        isLoading = false;
      }
    })} class="mt-8 space-y-6">
      <!-- Server Error Message -->
      {#if login.result?.error}
        <div class="rounded-md bg-red-50 p-4 border border-red-200">
          <div class="flex items-center">
            <svg class="w-5 h-5 text-red-400 mr-2" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
            </svg>
            <span class="text-sm text-red-800">{login.result.error}</span>
          </div>
        </div>
      {/if}

      <div class="space-y-4">
        <!-- Username Field -->
        <div>
          <input
            type="text"
            name="username"
            placeholder="Username"
            bind:value={username}
            oninput={(e) => validateField('username', (e.target as HTMLInputElement)?.value || '')}
            class="appearance-none relative block w-full px-3 py-3 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm transition-colors
              {fieldErrors.username ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : 'border-gray-300'}"
            disabled={isLoading}
          />
          {#if fieldErrors.username}
            <p class="mt-1 text-sm text-red-600">{fieldErrors.username}</p>
          {/if}
        </div>

        <!-- Password Field -->
        <div>
          <input
            type="password"
            name="password"
            placeholder="Password"
            bind:value={password}
            oninput={(e) => validateField('password', (e.target as HTMLInputElement)?.value || '')}
            class="appearance-none relative block w-full px-3 py-3 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm transition-colors
              {fieldErrors.password ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : 'border-gray-300'}"
            disabled={isLoading}
          />
          {#if fieldErrors.password}
            <p class="mt-1 text-sm text-red-600">{fieldErrors.password}</p>
          {/if}
        </div>
      </div>

      <button
        type="submit"
        disabled={isLoading}
        class="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
      >
        {#if isLoading}
          <div class="flex items-center">
            <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Signing in...
          </div>
        {:else}
          Sign in
        {/if}
      </button>
    </form>
  </div>
</div>
