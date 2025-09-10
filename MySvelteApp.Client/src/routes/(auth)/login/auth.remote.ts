import { form, getRequestEvent } from '$app/server';
import { postAuthLogin } from '$api/schema/sdk.gen';
import { redirect } from '@sveltejs/kit';

export const login = form(async (data) => {
    const username = data.get('username') as string;
    const password = data.get('password') as string;

    // Validate input
    if (!username || !password) {
        return {
            error: 'Please fill in all fields',
            username,
            missing: true
        };
    }

    try {
        // Call the backend API
        const result = await postAuthLogin({
            body: { username, password }
        }) as any;

        const token = result.data?.token;

        if (!token) {
            // Check if there's an error message in the response
            if (result.error?.message) {
                return {
                    error: result.error.message,
                    username
                };
            }
            return {
                error: 'Login succeeded but no token received',
                username
            };
        }

        // Get the request event to access cookies
        const { cookies } = getRequestEvent();

        // Set the token in a secure cookie
        cookies.set('authToken', token, {
            path: '/',
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            maxAge: 60 * 60 * 24, // 24 hours
            sameSite: 'lax'
        });

        // Return success - client will handle redirect
        return { success: true };
    } catch (err: any) {
        if (err.response) {
            try {
                const body = await err.response.json();
                return {
                    error: body.message || 'Login failed',
                    username
                };
            } catch (parseError) {
                return {
                    error: 'Login failed',
                    username
                };
            }
        } else {
            return {
                error: 'Network error occurred',
                username
            };
        }
    }
});
