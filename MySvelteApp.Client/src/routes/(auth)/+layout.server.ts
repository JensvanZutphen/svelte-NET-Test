import { authService } from "$api/authService";
import { redirect } from "@sveltejs/kit";
import type { LayoutServerLoad } from "./$types";

export const load = (async ({ cookies }) => {
    const token = cookies.get('authToken');

    if (token) {
        // Optionally validate token with backend here
        authService.setToken(token);
        throw redirect(302, '/');
    }
}) satisfies LayoutServerLoad;
