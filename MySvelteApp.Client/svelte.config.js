import adapter from '@sveltejs/adapter-node';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

/** @type {import('@sveltejs/kit').Config} */
const config = {
    // Consult https://svelte.dev/docs/kit/integrations
    // for more information about preprocessors
    preprocess: vitePreprocess(),

    compilerOptions: {
        experimental: {
            async: true // This enables async boundaries in markup
        }
    },

    kit: {
        adapter: adapter(),

        // Enable experimental features
        experimental: {
            // Enable remote functions for type-safe server communication
            remoteFunctions: true,

            // Enable built-in observability and tracing
            tracing: {
                server: true
            },

            // Enable instrumentation for custom tracing setup
            instrumentation: {
                server: true
            }
        },
        alias: {
            $lib: 'src/lib',
            $routes: 'src/routes',
            $server: 'src/server',
            $drizzle: 'drizzle',
            $api: 'src/api',
            $src: 'src',
        }
    }
};

export default config;