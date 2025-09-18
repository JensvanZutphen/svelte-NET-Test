import prettier from 'eslint-config-prettier';
import { includeIgnoreFile, fixupPluginRules } from '@eslint/compat';
import js from '@eslint/js';
import svelte from 'eslint-plugin-svelte';
import globals from 'globals';
import { fileURLToPath } from 'node:url';
import ts from 'typescript-eslint';
import deprecation from 'eslint-plugin-deprecation';
import svelteConfig from './svelte.config.js';

const gitignorePath = fileURLToPath(new URL('./.gitignore', import.meta.url));
const tsconfigRootDir = fileURLToPath(new URL('.', import.meta.url));

export default ts.config(
	{ ignores: ['api/**', 'src/lib/components/ui/**'] },
	includeIgnoreFile(gitignorePath),
	js.configs.recommended,
	...ts.configs.recommended,
	...svelte.configs.recommended,
	prettier,
	...svelte.configs.prettier,
	{
		languageOptions: {
			globals: { ...globals.browser, ...globals.node }
		},
		plugins: {
			deprecation: fixupPluginRules(deprecation)
		},
		rules: {
			// typescript-eslint strongly recommend that you do not use the no-undef lint rule on TypeScript projects.
			// see: https://typescript-eslint.io/troubleshooting/faqs/eslint/#i-get-errors-from-the-no-undef-rule-about-global-variables-not-being-defined-even-though-there-are-no-typescript-errors
			'no-undef': 'off',
			'svelte/no-navigation-without-resolve': ['error', { ignoreGoto: true, ignoreLinks: true }]
		}
	},
	{
		files: ['src/**/*.svelte', 'src/**/*.svelte.ts', 'src/**/*.svelte.js'],
		languageOptions: {
			parserOptions: {
				project: ['./tsconfig.json'],
				tsconfigRootDir,
				extraFileExtensions: ['.svelte'],
				parser: ts.parser,
				svelteConfig
			}
		},
			rules: {
				'deprecation/deprecation': 'error'
			}
		},
		{
			files: ['src/**/*.{ts,tsx}'],
			languageOptions: {
				parserOptions: {
					project: ['./tsconfig.json'],
					tsconfigRootDir
				}
			},
			rules: {
				'deprecation/deprecation': 'error'
			}
	}
);
