import { redirect } from '@sveltejs/kit';
import type { Actions } from '@sveltejs/kit';

export const actions: Actions = {
  default: async ({ cookies }) => {
    // Delete the auth token cookie
    cookies.delete('authToken', { path: '/' });
    
    // Redirect to login page
    return redirect(302, '/login');
  }
};