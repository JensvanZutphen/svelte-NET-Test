import { command, getRequestEvent } from '$app/server';

export const logout = command(async () => {
    // Get the request event to access cookies
    const { cookies } = getRequestEvent();

    // Delete the auth token cookie
    cookies.delete('authToken', { path: '/' });

    // Return success - client will handle redirect
    return { success: true };
});
