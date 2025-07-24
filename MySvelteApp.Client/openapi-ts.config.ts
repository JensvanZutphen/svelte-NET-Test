import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'http://localhost:7216/swagger/v1/swagger.json',
  output: 'src/api/schema',
  plugins: [
    '@hey-api/client-fetch',               // HTTP client plugin :contentReference[oaicite:3]{index=3}
    'zod',                                  // Zod schemas plugin :contentReference[oaicite:4]{index=4}
    '@hey-api/schemas',                     // JSON Schema objects (optional) :contentReference[oaicite:5]{index=5}
    {
      name: '@hey-api/sdk',                
      validator: true,                      // Enable Zod-based runtime validation :contentReference[oaicite:6]{index=6}
    },
  ],
  auth: () => localStorage.getItem('authToken') ?? '',
});
