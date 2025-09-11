// import { authService } from "$api/authService";
// import { redirect } from "@sveltejs/kit";
// import type { LayoutServerLoad } from "./$types";

// export const load = (async ({ cookies }) => {
//     const token = cookies.get('authToken');

//     if (!token) {
//         throw redirect(302, '/login');
//     }

//     // Optionally validate token with backend here
//     authService.setToken(token);
// }) satisfies LayoutServerLoad;