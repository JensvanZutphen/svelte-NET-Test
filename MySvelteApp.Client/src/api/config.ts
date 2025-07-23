// src/api/config.ts
import { dev } from '$app/environment';
import { PUBLIC_API_ENDPOINT } from '$env/static/public';

const defaultDevUrl  = 'http://localhost:7216';
const defaultProdUrl = 'https://api.yourdomain.com';

export const config = {
  apiEndpoint:
    // 1) if someone set the .env var, use it
    (PUBLIC_API_ENDPOINT && PUBLIC_API_ENDPOINT !== '')
      ? PUBLIC_API_ENDPOINT
      // 2) otherwise pick based on dev vs prod
      : (dev ? defaultDevUrl : defaultProdUrl)
};