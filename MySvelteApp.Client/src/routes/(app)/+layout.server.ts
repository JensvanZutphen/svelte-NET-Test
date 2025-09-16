// if the user is not authenticated, redirect to the login page
import { redirect } from '@sveltejs/kit';
import { logger } from '$lib/server/logger';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ cookies }) => {
    
    const token = cookies.get('auth_token');
    const log = logger;

    if (!token) {
        log.warn('User is not authenticated, redirecting to login page');
        return redirect(302, '/login');
    }

    // Verify token with backend
    try {
        const response = await fetch('http://localhost:7216/api/Auth/validate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            log.warn('Invalid token, redirecting to login page');
            cookies.delete('auth_token', { path: '/' });
            return redirect(302, '/login');
        }

        const result = await response.json();
        
        return {
            user: result.user
        };
    } catch (error) {
        log.error({ err: error }, 'Token validation failed, redirecting to login page');
        cookies.delete('auth_token', { path: '/' });
        return redirect(302, '/login');
    }
};
