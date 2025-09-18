import { form, command, query } from '$app/server';
import { getRequestEvent } from '$app/server';
import { error } from '@sveltejs/kit';
import { postAuthLogin, postAuthRegister } from '$api/schema/sdk.gen';
import { z } from 'zod';
import { extractUserFromToken, isTokenExpired, isValidTokenFormat } from '$lib/utils/jwt';
import { resolveAuthError } from '$lib/auth/error-messages';

// Stricter UI-side validation schemas for immediate feedback
const zLoginForm = z.object({
	username: z.string().trim().min(1, 'Username is required'),
	password: z.string().min(1, 'Password is required')
});

const zRegisterForm = z.object({
	username: z.string().trim().min(1, 'Username is required'),
	email: z.email('Valid email required'),
	password: z.string().min(8, 'Password must be at least 8 characters')
});

const zRegisterFormWithConfirm = zRegisterForm
	.extend({
		confirmPassword: z.string().min(1, 'Please confirm your password')
	})
	.superRefine((data, ctx) => {
		if (data.password !== data.confirmPassword) {
			ctx.addIssue({
				code: 'custom',
				message: 'Passwords do not match',
				path: ['confirmPassword']
			});
		}
	});

const getString = (value: FormDataEntryValue | null) => (typeof value === 'string' ? value : '');

// Login form handler with automatic validation
export const login = form(async (formData) => {
	const parsed = zLoginForm.safeParse({
		username: getString(formData.get('username')),
		password: getString(formData.get('password'))
	});

	if (!parsed.success) {
		throw error(400, { message: 'Invalid login data' });
	}

	const { username, password } = parsed.data;
	const { cookies } = getRequestEvent();

	try {
		// Use generated API client with ThrowOnError for cleaner control flow
		const response = await postAuthLogin({
			body: { username, password },
			throwOnError: true as const
		});

		const result = response.data;

		// Set JWT token in cookie
		if (result?.token) {
			cookies.set('auth_token', result.token, {
				path: '/',
				httpOnly: true,
				secure: import.meta.env.PROD,
				sameSite: 'strict'
			});
		}

		return result;
	} catch (err) {
		console.error('Login error:', err);
		const enhancedError = resolveAuthError(
			err,
			'Network error. Please check your connection and try again.'
		);
		throw error(enhancedError.statusCode || 401, { message: enhancedError.message });
	}
});

// Registration form handler with automatic validation
export const register = form(async (formData) => {
	const parsed = zRegisterFormWithConfirm.safeParse({
		username: getString(formData.get('username')),
		email: getString(formData.get('email')),
		password: getString(formData.get('password')),
		confirmPassword: getString(formData.get('confirmPassword'))
	});

	if (!parsed.success) {
		const message = parsed.error.issues[0]?.message ?? 'Invalid registration data';
		throw error(400, { message });
	}

	const { username, email, password } = parsed.data;
	const registerData = { username, email, password };
	try {
		// Use generated API client with ThrowOnError
		const response = await postAuthRegister({
			body: registerData,
			throwOnError: true as const
		});

		const result = response.data;

		return result;
	} catch (err) {
		console.error('Registration error:', err);
		const enhancedError = resolveAuthError(err, 'Registration failed');
		throw error(enhancedError.statusCode || 400, { message: enhancedError.message });
	}
});

// Logout command
export const logout = command(async () => {
	const { cookies } = getRequestEvent();

	// Clear auth token cookie
	cookies.delete('auth_token', { path: '/' });

	return { success: true };
});

// Get current user query
export const getCurrentUser = query(async () => {
	const { cookies } = getRequestEvent();

	const token = cookies.get('auth_token');
	if (!token) {
		error(401, 'Not authenticated');
	}

	// Validate token format
	if (!isValidTokenFormat(token)) {
		console.error('Invalid token format');
		error(401, 'Authentication failed');
	}

	// Check if token is expired
	if (isTokenExpired(token)) {
		console.error('Token expired');
		error(401, 'Authentication failed');
	}

	// Extract user data from JWT
	try {
		const user = extractUserFromToken(token);
		return {
			id: user.id,
			username: user.username,
			email: `${user.username}@example.com`, // Placeholder email since JWT doesn't include it
			token: user.token
		};
	} catch (err) {
		console.error('Token decoding error:', err);
		error(401, 'Authentication failed');
	}
});

// Check if user is authenticated
export const isAuthenticated = query(async () => {
	const { cookies } = getRequestEvent();
	const token = cookies.get('auth_token');

	if (!token) {
		return false;
	}

	// Validate token format and expiration
	try {
		return isValidTokenFormat(token) && !isTokenExpired(token);
	} catch {
		return false;
	}
});
