import type { ActionFailure } from '@sveltejs/kit';
import { zAuthErrorResponse } from '$api/schema/zod.gen';
import type { AuthErrorResponse } from '$api/schema/types.gen';

// Error type constants for consistency
export enum AuthErrorType {
	None = 0,
	Validation = 1,
	Conflict = 2,
	Unauthorized = 3
}

export interface EnhancedAuthError {
	message: string;
	type: AuthErrorType;
	statusCode?: number;
}

// Common error patterns and their user-friendly messages
const ERROR_PATTERNS = {
	usernameRequired: /username.*required/i,
	emailRequired: /email.*required/i,
	passwordRequired: /password.*required/i,
	passwordTooShort: /password.*8.*characters/i,
	passwordComplexity: /password.*uppercase.*lowercase.*number/i,
	usernameTaken: /username.*taken/i,
	emailTaken: /email.*registered/i,
	invalidCredentials: /invalid.*username.*password/i,
	tokenExpired: /token.*expired/i,
	tokenInvalid: /token.*invalid/i
} as const;

const extractAuthMessage = (payload: unknown): string | undefined => {
	const parsed = zAuthErrorResponse.safeParse(payload);
	if (!parsed.success) return undefined;
	const message = parsed.data.message;
	return typeof message === 'string' && message.trim().length > 0 ? message : undefined;
};

/**
 * Determine error type based on message content
 */
const determineErrorType = (message: string): AuthErrorType => {
	if (
		ERROR_PATTERNS.usernameRequired.test(message) ||
		ERROR_PATTERNS.emailRequired.test(message) ||
		ERROR_PATTERNS.passwordRequired.test(message) ||
		ERROR_PATTERNS.passwordTooShort.test(message) ||
		ERROR_PATTERNS.passwordComplexity.test(message)
	) {
		return AuthErrorType.Validation;
	}

	if (ERROR_PATTERNS.usernameTaken.test(message) || ERROR_PATTERNS.emailTaken.test(message)) {
		return AuthErrorType.Conflict;
	}

	if (
		ERROR_PATTERNS.invalidCredentials.test(message) ||
		ERROR_PATTERNS.tokenExpired.test(message) ||
		ERROR_PATTERNS.tokenInvalid.test(message)
	) {
		return AuthErrorType.Unauthorized;
	}

	return AuthErrorType.None;
};

/**
 * Enhanced error resolution with type classification and better fallbacks
 */
export const resolveAuthError = (
	err: unknown,
	fallback: string = 'An unexpected error occurred'
): EnhancedAuthError => {
	let message = fallback;
	let statusCode: number | undefined;
	let errorType = AuthErrorType.None;

	if (typeof err === 'object' && err !== null) {
		// Handle HTTP response errors
		if ('status' in err) {
			statusCode = (err as { status: number }).status;
		}

		// Extract from body
		if ('body' in err) {
			const extracted = extractAuthMessage((err as { body?: unknown }).body);
			if (extracted) {
				message = extracted;
				errorType = determineErrorType(message);
			}
		}

		// Extract from SvelteKit action failure
		if ('data' in err) {
			const failure = err as ActionFailure<AuthErrorResponse>;
			const extracted = extractAuthMessage(failure.data);
			if (extracted) {
				message = extracted;
				errorType = determineErrorType(message);
			}
			if (failure.status) {
				statusCode = failure.status;
			}
		}
	}

	// Handle standard Error objects
	if (err instanceof Error) {
		const errorMessage = err.message?.trim();
		if (errorMessage) {
			message = errorMessage;
			errorType = determineErrorType(message);
		}
	}

	return {
		message,
		type: errorType,
		statusCode
	};
};

/**
 * Legacy function for backward compatibility
 */
export const resolveAuthErrorMessage = (err: unknown, fallback: string): string => {
	return resolveAuthError(err, fallback).message;
};

/**
 * Get user-friendly error message based on error type
 */
export const getUserFriendlyMessage = (error: EnhancedAuthError): string => {
	switch (error.type) {
		case AuthErrorType.Validation:
			return error.message; // Validation messages are usually clear enough
		case AuthErrorType.Conflict:
			return 'This information is already in use. Please try something different.';
		case AuthErrorType.Unauthorized:
			return 'Authentication failed. Please check your credentials and try again.';
		default:
			return error.message;
	}
};
