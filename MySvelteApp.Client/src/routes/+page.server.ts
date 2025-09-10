import { redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ cookies }) => {
  // Check if user has an auth token
  const authToken = cookies.get('authToken');

  if (authToken) {
    // User is authenticated, redirect to pokemon page
    throw redirect(302, '/pokemon');
  } else {
    // User is not authenticated, redirect to login
    throw redirect(302, '/login');
  }
};
