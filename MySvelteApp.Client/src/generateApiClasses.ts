import { config } from "./api/config.js";
import { execSync } from "child_process";

const openApiPath = `${config.apiEndpoint}/swagger/v1/swagger.json`;
const zodOut = "./src/api/api-zod-client.ts";

console.log("Generating Zod client at", openApiPath);
execSync(
  `npx openapi-zod-client "${openApiPath}" -o "${zodOut}"`,
  { stdio: "inherit" }
);
console.log("Generated Zod client at", zodOut);
