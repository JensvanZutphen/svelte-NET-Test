# Login and Register Pages Refactoring Plan

## Current Implementation Analysis

The current authentication system uses client-side form handling in Svelte components with direct API calls to the backend. The implementation includes:

1. **Login Page** (`MySvelteApp.Client/src/routes/(auth)/login/+page.svelte`):
   - Client-side form handling with `handleLogin()` function
   - Uses `postAuthLogin` from the generated SDK to communicate with backend
   - Manages form state (username, password, error, loading)
   - Uses `authService` to store JWT token in localStorage
   - Uses `goto` for navigation after successful login

2. **Register Page** (`MySvelteApp.Client/src/routes/(auth)/register/+page.svelte`):
   - Client-side form handling with `handleRegister()` function
   - Uses `postAuthRegister` from the generated SDK to communicate with backend
   - Manages form state (username, email, password, error, loading)
   - Uses `authService` to store JWT token in localStorage
   - Uses `goto` for navigation after successful registration

3. **Authentication Service** (`MySvelteApp.Client/src/api/authService.ts`):
   - Manages JWT token in localStorage
   - Provides methods for checking authentication status
   - Adds authorization headers to API requests

4. **Backend Implementation**:
   - ASP.NET Core controllers with `/Auth/register` and `/Auth/login` endpoints
   - JWT token generation and validation
   - Password hashing with HMACSHA512

## Refactoring Goals

Refactor the authentication pages to use SvelteKit's form actions pattern:

1. Move authentication logic to server-side `+page.server.ts` files
2. Use SvelteKit's form actions for handling form submissions
3. Implement proper error handling with `fail()` and `redirect()`
4. Use progressive enhancement for better UX
5. Maintain the same authentication flow and user experience

## Implementation Plan

### 1. Create `+page.server.ts` files

Create server files for both login and register routes that will contain the form actions.

#### Login Page Server File
File: `MySvelteApp.Client/src/routes/(auth)/login/+page.server.ts`

```typescript
import { fail, redirect } from '@sveltejs/kit';
import { postAuthLogin } from '$api/schema/sdk.gen';
import { authService } from '$api/authService';
import type { Actions } from './$types';

export const actions = {
  default: async ({ request, cookies }) => {
    const data = await request.formData();
    const username = data.get('username') as string;
    const password = data.get('password') as string;

    // Validate input
    if (!username || !password) {
      return fail(400, { 
        error: 'Please fill in all fields',
        username,
        missing: true
      });
    }

    try {
      // Call the backend API
      const result = await postAuthLogin({
        body: { username, password }
      }) as any;

      const token = result.token;
      
      // Set the token in a secure cookie
      cookies.set('authToken', token, {
        path: '/',
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        maxAge: 60 * 60 * 24 // 24 hours
      });

      // Redirect to the main app
      throw redirect(302, '/pokemon');
    } catch (err: any) {
      console.error('Login error:', err);
      
      if (err.response) {
        try {
          const body = await err.response.json();
          return fail(err.response.status, { 
            error: body.message || 'Login failed',
            username
          });
        } catch {
          return fail(err.response.status, { 
            error: 'Login failed',
            username
          });
        }
      } else {
        return fail(500, { 
          error: 'Network error occurred',
          username
        });
      }
    }
  }
} satisfies Actions;
```

#### Register Page Server File
File: `MySvelteApp.Client/src/routes/(auth)/register/+page.server.ts`

```typescript
import { fail, redirect } from '@sveltejs/kit';
import { postAuthRegister } from '$api/schema/sdk.gen';
import type { Actions } from './$types';

export const actions = {
  default: async ({ request, cookies }) => {
    const data = await request.formData();
    const username = data.get('username') as string;
    const email = data.get('email') as string;
    const password = data.get('password') as string;

    // Validate input
    if (!username || !email || !password) {
      return fail(400, { 
        error: 'Please fill in all fields',
        username,
        email,
        missing: true
      });
    }

    try {
      // Call the backend API
      const result = await postAuthRegister({
        body: { username, email, password }
      }) as any;

      const token = result.token;
      
      // Set the token in a secure cookie
      cookies.set('authToken', token, {
        path: '/',
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        maxAge: 60 * 60 * 24 // 24 hours
      });

      // Redirect to the main app
      throw redirect(302, '/pokemon');
    } catch (err: any) {
      console.error('Register error:', err);
      
      if (err.response) {
        try {
          const body = await err.response.json();
          return fail(err.response.status, { 
            error: body.message || 'Registration failed',
            username,
            email
          });
        } catch {
          return fail(err.response.status, { 
            error: 'Registration failed',
            username,
            email
          });
        }
      } else {
        return fail(500, { 
          error: 'Network error occurred',
          username,
          email
        });
      }
    }
  }
} satisfies Actions;
```

### 2. Update Svelte Components

Update the Svelte components to use form actions with progressive enhancement.

#### Login Page Component
File: `MySvelteApp.Client/src/routes/(auth)/login/+page.svelte`

```svelte
<script lang="ts">
  import { enhance } from '$app/forms';
  import type { ActionData } from './$types';

  export let form: ActionData;
  
  // Preserve form values after failed submission
  let username = form?.username || '';
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
    
    <form method="POST" use:enhance class="mt-8 space-y-6">
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
```

#### Register Page Component
File: `MySvelteApp.Client/src/routes/(auth)/register/+page.svelte`

```svelte
<script lang="ts">
  import { enhance } from '$app/forms';
  import type { ActionData } from './$types';

  export let form: ActionData;
  
  // Preserve form values after failed submission
  let username = form?.username || '';
  let email = form?.email || '';
</script>

<div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full space-y-8">
    <div>
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Sign up for an account
      </h2>
      <p class="mt-2 text-center text-sm text-gray-600">
        Already have an account?
        <a href="/login" class="font-medium text-indigo-600 hover:text-indigo-500">
          Sign in
        </a>
      </p>
    </div>
    
    <form method="POST" use:enhance class="mt-8 space-y-6">
      <div class="rounded-md shadow-sm -space-y-px">
        <input
          type="text"
          name="username"
          placeholder="Username"
          bind:value={username}
          class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
        />
        <input
          type="email"
          name="email"
          placeholder="Email"
          bind:value={email}
          class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
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
        Sign up
      </button>
    </form>
  </div>
</div>
```

### 3. Update Authentication Service

Update the authentication service to work with cookies instead of localStorage.

File: `MySvelteApp.Client/src/api/authService.ts`

```typescript
// src/api/authService.ts
class AuthService {
  private token: string | null = null;

  constructor() {
    // In browser environment, token will be managed via cookies
    // Server-side rendering will handle token via load functions
  }

  // Get the current auth token (this would typically come from a load function)
  getToken(): string | null {
    return this.token;
  }

  // Check if user is authenticated (this would typically come from a load function)
  isAuthenticated(): boolean {
    return !!this.token;
  }

  // Set the auth token (primarily for server-side use)
  setToken(token: string): void {
    this.token = token;
  }

  // Clear the auth token (primarily for server-side use)
  clearToken(): void {
    this.token = null;
  }

  // Get auth headers for API requests
  getAuthHeaders(): HeadersInit {
    if (this.token) {
      return {
        'Authorization': `Bearer ${this.token}`,
        'Content-Type': 'application/json',
      };
    }
    return {
      'Content-Type': 'application/json',
    };
  }

  // Intercept API calls to add auth headers
  async fetchWithAuth(url: string, options: RequestInit = {}): Promise<Response> {
    const headers = {
      ...this.getAuthHeaders(),
      ...options.headers,
    };

    return fetch(url, {
      ...options,
      headers,
    });
  }
}

export const authService = new AuthService();
```

### 4. Update Layout Server Files

Update the layout server files to check authentication status using cookies.

File: `MySvelteApp.Client/src/routes/(app)/+layout.server.ts`

```typescript
import { authService } from "$api/authService";
import { redirect } from "@sveltejs/kit";
import type { LayoutServerLoad } from "./$types";

export const load = (async ({ cookies }) => {
    const token = cookies.get('authToken');
    
    if (!token) {
        throw redirect(302, '/login');
    }
    
    // Optionally validate token with backend here
    authService.setToken(token);
}) satisfies LayoutServerLoad;
```

File: `MySvelteApp.Client/src/routes/(auth)/+layout.server.ts`

```typescript
import { authService } from "$api/authService";
import { redirect } from "@sveltejs/kit";
import type { LayoutServerLoad } from "./$types";

export const load = (async ({ cookies }) => {
    const token = cookies.get('authToken');
    
    if (token) {
        // Optionally validate token with backend here
        authService.setToken(token);
        throw redirect(302, '/');
    }
}) satisfies LayoutServerLoad;
```

## Benefits of This Refactoring

1. **Better Security**: Using httpOnly cookies instead of localStorage for JWT tokens
2. **Progressive Enhancement**: Forms work even without JavaScript
3. **Server-Side Validation**: Form validation happens on the server
4. **Better Error Handling**: Proper error states with preserved form data
5. **SvelteKit Best Practices**: Following the recommended form actions pattern
6. **Improved UX**: Better loading states and error messaging

## Implementation Steps

1. Create the `+page.server.ts` files for login and register routes
2. Update the Svelte components to use form actions
3. Update the authentication service to work with cookies
4. Update layout server files to check authentication status
5. Test the refactored authentication flow