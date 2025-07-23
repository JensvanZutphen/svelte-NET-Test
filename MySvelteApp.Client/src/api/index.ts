import { createApiClient, schemas as _schemas } from "./api-zod-client";
import { config } from "./config";

export const api = createApiClient(config.apiEndpoint);
export const schemas = _schemas;
