import adapter from '@sveltejs/adapter-node';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

/** @type {import('@sveltejs/kit').Config} */
const config = {
    // Consult https://kit.svelte.dev/docs/integrations#preprocessors
    // for more information about preprocessors
    preprocess: vitePreprocess(),
    compilerOptions: {
        experimental: {
            async: true
        }
    },
    kit: {
        adapter: adapter(),
        alias: { $api: './src/api',
            $static: './static',
            $src: './src',
            $env: './src/env',
            $app: './src/app',
            $lib: './src/lib',
            $routes: './src/routes',
            $components: './src/components',
            $utils: './src/utils',
            $states: './src/states',
         }
    }
};

export default config;