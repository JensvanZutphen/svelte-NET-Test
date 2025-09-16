import { form, command, query } from '$app/server';
import { getRequestEvent } from '$app/server';
import { error } from '@sveltejs/kit';
import { postAuthLogin, postAuthRegister, getTestAuth } from '$api/schema/sdk.gen';
import {
	zLoginRequest,
	zRegisterRequest,
	zAuthErrorResponse,
} from '$api/schema/zod.gen';
import type { LoginRequest, RegisterRequest } from '$api/schema/types.gen';

// Login form handler with automatic validation
export const login = form(async (data) => {
	// Validate form data with Zod
	const formData: LoginRequest = {
		username: data.get('username') as string,
		password: data.get('password') as string,
	};

	const validationResult = zLoginRequest.safeParse(formData);
	if (!validationResult.success) {
		error(400, 'Invalid login data');
	}
	
	const loginData = validationResult.data;
	const { cookies } = getRequestEvent();
	
	try {
		// Use generated API client with ThrowOnError for cleaner control flow
		const response = await postAuthLogin({
			body: loginData,
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
		if (err instanceof Error) {
			const parsed = zAuthErrorResponse.safeParse((err as unknown) as object);
			const message = parsed.success && parsed.data.message ? parsed.data.message : err.message;
			error(401, message);
		}
		const parsed = zAuthErrorResponse.safeParse(err);
		const message = parsed.success && parsed.data.message ? parsed.data.message : 'Network error. Please check your connection and try again.';
		error(401, message);
	}
});

// Registration form handler with automatic validation
export const register = form(async (data) => {

	// Validate passwords match
	const confirmPassword = data.get('confirmPassword');
	if (!confirmPassword || confirmPassword !== data.get('password')) {
		error(400, 'Passwords do not match');
	}

	// Validate form data with Zod
	const formData: RegisterRequest = {
		username: data.get('username') as string,
		email: data.get('email') as string,
		password: data.get('password') as string,
	};

	const validationResult = zRegisterRequest.safeParse(formData);
	if (!validationResult.success) {
		error(400, 'Invalid registration data');
	}
	
	const registerData = validationResult.data;
	
	try {
		// Use generated API client with ThrowOnError
		const response = await postAuthRegister({
			body: registerData,
			throwOnError: true as const
		});

		const result = response.data;

		return result;
	} catch (err) {
		console.log('Registration catch error:', err);
		if (err instanceof Error) {
			const parsed = zAuthErrorResponse.safeParse((err as unknown) as object);
			const message = parsed.success && parsed.data.message ? parsed.data.message : err.message;
			error(400, message);
		}
		const parsed = zAuthErrorResponse.safeParse(err);
		const message = parsed.success && parsed.data.message ? parsed.data.message : 'Registration failed';
		error(400, message);
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

	// Validate token with backend using generated TestAuth client
	try {
		const response = await getTestAuth({
			headers: {
				'Authorization': `Bearer ${token}`
			},
			throwOnError: true as const
		});

		// Return a mock user since the test auth doesn't return user details
		return {
			id: 'user123',
			email: 'user@example.com',
			name: 'Test User',
			token
		};
	} catch (err) {
		console.error('Token validation error:', err);
		error(401, 'Authentication failed');
	}
});

// Check if user is authenticated
export const isAuthenticated = query(async () => {
	const { cookies } = getRequestEvent();
	const token = cookies.get('auth_token');
	return !!token;
});