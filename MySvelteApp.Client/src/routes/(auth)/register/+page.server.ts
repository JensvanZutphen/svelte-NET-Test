import { fail, redirect } from '@sveltejs/kit';
import { postAuthRegister } from '$api/schema/sdk.gen';
import type { Actions } from '@sveltejs/kit';

export const actions: Actions = {
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

      const token = result.data?.token;

      if (!token) {
        // Check if there's an error message in the response
        if (result.error?.message) {
          return fail(400, {
            error: result.error.message,
            username,
            email
          });
        }
        return fail(500, {
          error: 'Registration succeeded but no token received',
          username,
          email
        });
      }

      // Set the token in a secure cookie
      cookies.set('authToken', token, {
        path: '/',
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        maxAge: 60 * 60 * 24, // 24 hours
        sameSite: 'lax'
      });

      // Return success indicator instead of redirect
      return {
        success: true,
        token: token
      };
    } catch (err: any) {
      if (err.response) {
        try {
          const body = await err.response.json();
          return fail(err.response.status, {
            error: body.message || 'Registration failed',
            username,
            email
          });
        } catch (parseError) {
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
};