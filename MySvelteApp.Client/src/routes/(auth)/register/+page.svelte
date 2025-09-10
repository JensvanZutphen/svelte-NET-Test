<script lang="ts">
  import { register } from './auth.remote';
  import { goto } from '$app/navigation';

  // Form state with runes
  let username = $state('');
  let email = $state('');
  let password = $state('');
  let confirmPassword = $state('');
  let isLoading = $state(false);
  let fieldErrors = $state({
    username: '',
    email: '',
    password: '',
    confirmPassword: ''
  });

  // Validate fields
  const validateField = (field: keyof typeof fieldErrors, value: string) => {
    switch (field) {
      case 'username':
        if (!value.trim()) {
          fieldErrors.username = 'Username is required';
        } else if (value.length < 3) {
          fieldErrors.username = 'Username must be at least 3 characters';
        } else if (!/^[a-zA-Z0-9_]+$/.test(value)) {
          fieldErrors.username = 'Username can only contain letters, numbers, and underscores';
        } else {
          fieldErrors.username = '';
        }
        break;
      case 'email':
        if (!value.trim()) {
          fieldErrors.email = 'Email is required';
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
          fieldErrors.email = 'Please enter a valid email address';
        } else {
          fieldErrors.email = '';
        }
        break;
      case 'password':
        if (!value) {
          fieldErrors.password = 'Password is required';
        } else if (value.length < 8) {
          fieldErrors.password = 'Password must be at least 8 characters';
        } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(value)) {
          fieldErrors.password = 'Password must contain at least one uppercase letter, one lowercase letter, and one number';
        } else {
          fieldErrors.password = '';
          // Re-validate confirm password when password changes
          if (confirmPassword) {
            validateField('confirmPassword', confirmPassword);
          }
        }
        break;
      case 'confirmPassword':
        if (!value) {
          fieldErrors.confirmPassword = 'Please confirm your password';
        } else if (value !== password) {
          fieldErrors.confirmPassword = 'Passwords do not match';
        } else {
          fieldErrors.confirmPassword = '';
        }
        break;
    }
  };

  // Validate form before submission
  const validateForm = () => {
    // Validate all fields
    validateField('username', username);
    validateField('email', email);
    validateField('password', password);
    validateField('confirmPassword', confirmPassword);

    // Check if there are any field errors
    return !Object.values(fieldErrors).some(error => error !== '');
  };
</script>

<div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full space-y-8">
    <div>
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Sign up for an account
      </h2>
      <p class="mt-2 text-center text-sm text-gray-600">
        Already have an account?
        <a href="/login" class="font-medium text-indigo-600 hover:text-indigo-500 transition-colors">
          Sign in
        </a>
      </p>
    </div>

    <form {...register.enhance(async ({ form, submit }) => {
      // Validate form before submission
      if (!validateForm()) {
        return; // Don't submit if validation fails
      }

      isLoading = true;
      try {
        await submit();
        // Check result after submission
        if (register.result?.success) {
          console.log('Registration successful, navigating to /pokemon');
          await goto('/pokemon');
        }
      } catch (error) {
        console.error('Registration error:', error);
      } finally {
        isLoading = false;
      }
    })} class="mt-8 space-y-6">
      <!-- Server Error Message -->
      {#if register.result?.error}
        <div class="rounded-md bg-red-50 p-4 border border-red-200">
          <div class="flex items-center">
            <svg class="w-5 h-5 text-red-400 mr-2" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
            </svg>
            <span class="text-sm text-red-800">{register.result.error}</span>
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

        <!-- Email Field -->
        <div>
          <input
            type="email"
            name="email"
            placeholder="Email address"
            bind:value={email}
            oninput={(e) => validateField('email', (e.target as HTMLInputElement)?.value || '')}
            class="appearance-none relative block w-full px-3 py-3 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm transition-colors
              {fieldErrors.email ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : 'border-gray-300'}"
            disabled={isLoading}
          />
          {#if fieldErrors.email}
            <p class="mt-1 text-sm text-red-600">{fieldErrors.email}</p>
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
          {:else}
            <p class="mt-1 text-xs text-gray-500">Password must be at least 8 characters with uppercase, lowercase, and numbers</p>
          {/if}
        </div>

        <!-- Confirm Password Field -->
        <div>
          <input
            type="password"
            name="confirmPassword"
            placeholder="Confirm password"
            bind:value={confirmPassword}
            oninput={(e) => validateField('confirmPassword', (e.target as HTMLInputElement)?.value || '')}
            class="appearance-none relative block w-full px-3 py-3 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm transition-colors
              {fieldErrors.confirmPassword ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : 'border-gray-300'}"
            disabled={isLoading}
          />
          {#if fieldErrors.confirmPassword}
            <p class="mt-1 text-sm text-red-600">{fieldErrors.confirmPassword}</p>
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
            Creating account...
          </div>
        {:else}
          Sign up
        {/if}
      </button>
    </form>
  </div>
</div>
