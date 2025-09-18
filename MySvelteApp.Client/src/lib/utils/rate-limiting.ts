import { browser } from '$app/environment';

export interface RateLimitConfig {
	/** Maximum number of attempts allowed */
	maxAttempts: number;
	/** Time window in milliseconds */
	windowMs: number;
	/** Key for storing attempts (should be unique per action) */
	key: string;
}

export interface RateLimitResult {
	/** Whether the action is allowed */
	allowed: boolean;
	/** Number of attempts remaining */
	remaining: number;
	/** Time until reset in milliseconds */
	resetIn: number;
	/** Whether rate limit was exceeded */
	exceeded: boolean;
}

/**
 * Client-side rate limiting utility using localStorage
 */
export class RateLimiter {
	private config: RateLimitConfig;

	constructor(config: RateLimitConfig) {
		this.config = config;
	}

	/**
	 * Check if an action is allowed based on rate limiting rules
	 */
	checkLimit(): RateLimitResult {
		if (!browser) {
			// Server-side: always allow
			return { allowed: true, remaining: this.config.maxAttempts, resetIn: 0, exceeded: false };
		}

		const now = Date.now();
		const key = `ratelimit_${this.config.key}`;
		const stored = localStorage.getItem(key);

		let attempts: number[] = [];

		if (stored) {
			try {
				attempts = JSON.parse(stored);
				// Filter out old attempts outside the window
				attempts = attempts.filter((timestamp) => now - timestamp < this.config.windowMs);
			} catch {
				attempts = [];
			}
		}

		const remaining = Math.max(0, this.config.maxAttempts - attempts.length);
		const allowed = attempts.length < this.config.maxAttempts;

		let resetIn = 0;
		if (attempts.length > 0) {
			const oldestAttempt = Math.min(...attempts);
			resetIn = Math.max(0, this.config.windowMs - (now - oldestAttempt));
		}

		return {
			allowed,
			remaining,
			resetIn,
			exceeded: !allowed
		};
	}

	/**
	 * Record an attempt
	 */
	recordAttempt(): void {
		if (!browser) return;

		const now = Date.now();
		const key = `ratelimit_${this.config.key}`;
		const stored = localStorage.getItem(key);

		let attempts: number[] = [];

		if (stored) {
			try {
				attempts = JSON.parse(stored);
			} catch {
				attempts = [];
			}
		}

		// Add current attempt and filter out old ones
		attempts.push(now);
		attempts = attempts.filter((timestamp) => now - timestamp < this.config.windowMs);

		localStorage.setItem(key, JSON.stringify(attempts));
	}

	/**
	 * Clear all attempts (useful for successful actions)
	 */
	clearAttempts(): void {
		if (!browser) return;

		const key = `ratelimit_${this.config.key}`;
		localStorage.removeItem(key);
	}

	/**
	 * Get formatted time until reset
	 */
	getFormattedResetTime(): string {
		const result = this.checkLimit();
		if (result.resetIn === 0) return '';

		const seconds = Math.ceil(result.resetIn / 1000);
		if (seconds < 60) return `${seconds} second${seconds === 1 ? '' : 's'}`;

		const minutes = Math.ceil(seconds / 60);
		return `${minutes} minute${minutes === 1 ? '' : 's'}`;
	}
}

// Pre-configured rate limiters for common auth actions
export const loginRateLimiter = new RateLimiter({
	maxAttempts: 5,
	windowMs: 5 * 60 * 1000, // 5 minutes
	key: 'login'
});

export const registerRateLimiter = new RateLimiter({
	maxAttempts: 3,
	windowMs: 10 * 60 * 1000, // 10 minutes
	key: 'register'
});

export const passwordResetRateLimiter = new RateLimiter({
	maxAttempts: 3,
	windowMs: 60 * 60 * 1000, // 1 hour
	key: 'password-reset'
});

/**
 * Generic rate limiting function for forms
 */
export function withRateLimit(
	rateLimiter: RateLimiter,
	action: () => Promise<any>
): Promise<{ success: boolean; error?: string; data?: any }> {
	return new Promise((resolve) => {
		const limit = rateLimiter.checkLimit();

		if (!limit.allowed) {
			const resetTime = rateLimiter.getFormattedResetTime();
			resolve({
				success: false,
				error: `Too many attempts. Please try again in ${resetTime}.`
			});
			return;
		}

		// Record the attempt
		rateLimiter.recordAttempt();

		// Execute the action
		action()
			.then((data) => {
				// Clear attempts on success
				rateLimiter.clearAttempts();
				resolve({ success: true, data });
			})
			.catch((error) => {
				resolve({ success: false, error: error.message || 'An error occurred' });
			});
	});
}
