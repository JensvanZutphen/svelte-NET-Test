import { jwtDecode } from 'jwt-decode';

export interface JwtPayload {
	sub: string; // Subject (user ID)
	name: string; // Username
	nameid: string; // User ID (alternative claim)
	jti: string; // JWT ID
	iat: number; // Issued at
	exp: number; // Expiration time
	iss: string; // Issuer
	aud: string; // Audience
}

export interface DecodedUser {
	id: number;
	username: string;
	token: string;
}

/**
 * Decode and validate a JWT token
 */
export function decodeJwt(token: string): JwtPayload {
	try {
		return jwtDecode<JwtPayload>(token);
	} catch {
		throw new Error('Invalid JWT token');
	}
}

/**
 * Validate if a JWT token is expired
 */
export function isTokenExpired(token: string): boolean {
	try {
		const decoded = decodeJwt(token);
		const currentTime = Math.floor(Date.now() / 1000);
		return decoded.exp < currentTime;
	} catch {
		return true; // Consider invalid tokens as expired
	}
}

/**
 * Extract user information from JWT token
 */
export function extractUserFromToken(token: string): DecodedUser {
	const decoded = decodeJwt(token);

	return {
		id: parseInt(decoded.sub || decoded.nameid, 10),
		username: decoded.name,
		token
	};
}

/**
 * Validate token format and basic structure
 */
export function isValidTokenFormat(token: string): boolean {
	if (!token || typeof token !== 'string') {
		return false;
	}

	const parts = token.split('.');
	return parts.length === 3 && parts.every(part => part.length > 0);
}
