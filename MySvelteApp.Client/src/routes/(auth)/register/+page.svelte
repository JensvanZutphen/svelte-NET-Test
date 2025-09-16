<script lang="ts">
	import { goto } from '$app/navigation';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import { register } from '$lib/auth.remote';
	
	let isSubmitting = false;
	let error = '';
	let success = '';
</script>

<div class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8">
	<div class="max-w-md w-full space-y-8">
		<div class="text-center">
			<h2 class="mt-6 text-3xl font-bold text-gray-900 dark:text-white">
				Create your account
			</h2>
			<p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
				Already have an account?
				<a href="/login" class="font-medium text-blue-600 hover:text-blue-500 dark:text-blue-400">
					Sign in
				</a>
			</p>
		</div>

		<Card>
			<CardHeader class="space-y-1">
				<CardTitle class="text-2xl font-bold text-center">Get started</CardTitle>
				<CardDescription class="text-center">
					Create a new account to access all features
				</CardDescription>
			</CardHeader>
			<CardContent>
				<!-- Error and Success Messages -->
				{#if error}
					<div class="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
						{error}
					</div>
				{/if}
				
				{#if success}
					<div class="mb-4 p-3 bg-green-100 border border-green-400 text-green-700 rounded">
						{success}
					</div>
				{/if}
				
				<form {...register.enhance(async ({ form, data, submit }) => {
		error = '';
		success = '';
		isSubmitting = true;
		
		try {
			const result = await submit();
			success = 'Registration successful! Please log in.';
			setTimeout(() => goto('/login'), 2000);
		} catch (err: any) {
			error = err.message || 'Registration failed. Please try again.';
		} finally {
			isSubmitting = false;
		}
	})} class="space-y-4">
					<div class="space-y-2">
						<Label for="name">Full name</Label>
						<Input
							id="name"
							name="name"
							type="text"
							placeholder="Enter your full name"
							required
						/>
					</div>

					<div class="space-y-2">
						<Label for="email">Email address</Label>
						<Input
							id="email"
							name="email"
							type="email"
							placeholder="Enter your email"
							required
						/>
					</div>

					<div class="space-y-2">
						<Label for="password">Password</Label>
						<Input
							id="password"
							name="password"
							type="password"
							placeholder="Create a password"
							required
						/>
					</div>

					<div class="space-y-2">
						<Label for="confirmPassword">Confirm password</Label>
						<Input
							id="confirmPassword"
							name="confirmPassword"
							type="password"
							placeholder="Confirm your password"
							required
						/>
					</div>

					<Button type="submit" class="w-full" disabled={isSubmitting}>
						{isSubmitting ? 'Creating account...' : 'Create account'}
					</Button>

					<div class="text-center text-xs text-gray-500 dark:text-gray-400">
						By creating an account, you agree to our
						<a href="/terms" class="text-blue-600 hover:text-blue-500 dark:text-blue-400">
							Terms of Service
						</a>
						and
						<a href="/privacy" class="text-blue-600 hover:text-blue-500 dark:text-blue-400">
							Privacy Policy
						</a>
					</div>
				</form>
			</CardContent>
		</Card>
	</div>
</div>