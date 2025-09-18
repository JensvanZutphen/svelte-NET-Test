// if the user is not authenticated, redirect to the login page
import { redirect } from '@sveltejs/kit';
import { logger } from '$lib/server/logger';
import {
	decodeJwt,
	extractUserFromToken,
	isTokenExpired,
	isValidTokenFormat
} from '$lib/utils/jwt';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ cookies }) => {
	const token = cookies.get('auth_token');
	const log = logger;

	if (!token) {
		log.warn('User is not authenticated, redirecting to login page');
		return redirect(302, '/login');
	}

	// Validate token format
	if (!isValidTokenFormat(token)) {
		log.warn('Invalid token format, redirecting to login page');
		cookies.delete('auth_token', { path: '/' });
		return redirect(302, '/login');
	}

	// Check if token is expired
	if (isTokenExpired(token)) {
		log.warn('Token expired, redirecting to login page');
		cookies.delete('auth_token', { path: '/' });
		return redirect(302, '/login');
	}

	// Extract user data from JWT
	try {
		const user = extractUserFromToken(token);
		return {
			user: {
				id: user.id,
				name: user.username
			}
		};
	} catch (error) {
		log.error({ err: error }, 'Token decoding failed, redirecting to login page');
		cookies.delete('auth_token', { path: '/' });
		return redirect(302, '/login');
	}
};
