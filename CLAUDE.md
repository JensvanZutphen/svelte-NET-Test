# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Common Development Tasks
- `npm run dev` - Start both client and server in development mode (concurrently)
- `npm run build` - Build for production (client + server)
- `npm run test` - Run all tests (root level, currently not configured)
- `npm run docker:dev` - Start development containers
- `npm run docker:prod` - Start production containers

### Client-Specific Commands (run from `/MySvelteApp.Client/`)
- `npm run dev` - Start SvelteKit dev server (port 5173)
- `npm run build` - Build SvelteKit for production
- `npm run check` - TypeScript type checking
- `npm run lint` - Run ESLint and Prettier checks
- `npm run format` - Format code with Prettier
- `npm run test:unit` - Run Vitest unit tests
- `npm run test:e2e` - Run Playwright E2E tests
- `npm run generate-api-classes` - Generate TypeScript API client from OpenAPI spec

### Server-Specific Commands (run from `/MySvelteApp.Server/`)
- `dotnet run` - Start .NET Web API (port 7216)
- `dotnet build` - Build .NET project
- `dotnet test` - Run .NET tests

## Architecture Overview

This is a full-stack web application with SvelteKit frontend and .NET 9.0 backend:

### Frontend (SvelteKit)
- **Location**: `/MySvelteApp.Client/`
- **Framework**: SvelteKit 2.37.1 with Svelte 5.38.8
- **Build**: Vite 6.1.0
- **Styling**: Tailwind CSS 4.0 with shadcn/ui components
- **Testing**: Vitest (unit) + Playwright (E2E)
- **Experimental Features**: Async boundaries, remote functions, tracing, instrumentation

### Backend (.NET Web API)
- **Location**: `/MySvelteApp.Server/`
- **Framework**: .NET 9.0 with ASP.NET Core Web API
- **Database**: Entity Framework Core with in-memory database
- **Authentication**: JWT-based with HMACSHA512 password hashing
- **API Documentation**: Swagger/OpenAPI integration

## Key Project Structure

### Routes
- `(auth)/` - Authentication routes (login, register)
- `(app)/` - Protected application routes (dashboard, pokemon)
- API endpoints: `/Auth/*`, `/WeatherForecast`, `/RandomPokemon`, `/TestAuth`

### Component Organization
- `/src/lib/components/ui/` - shadcn/ui components
- `/src/lib/components/` - Custom components (sidebar, navigation, etc.)
- `/src/api/` - Generated API client code
- `/src/hooks/` - SvelteKit server hooks

### Server Structure
- `/Controllers/` - API controllers with authentication
- `/Models/` - Data models and DTOs
- `/Services/` - Business logic services
- `/Data/` - Database context

## Development Environment

### Ports
- **Client**: 5173 (Vite dev server)
- **API**: 7216 (ASP.NET Core)
- **Grafana**: 3000 (Observability)
- **Loki**: 3100 (Log aggregation)

### Dev Container
- Pre-configured with .NET 9.0 and Node.js
- VS Code extensions and debugging setup
- Integrated observability stack

## Authentication System

- JWT-based authentication with secure password hashing
- Global authorization policy
- Public endpoints marked with `[AllowAnonymous]`
- Protected routes in `(app)` layout group
- Server-side authentication in `/src/routes/(auth)/+layout.server.ts`

## Build & Deployment

### Production Build Process
1. Build SvelteKit client (`npm run build` in client directory)
2. Build .NET server with static assets
3. Multi-stage Docker image with Nginx reverse proxy
4. Container orchestration with Docker Compose

### API Client Generation
- Uses `@hey-api/openapi-ts` to generate TypeScript client from OpenAPI spec
- Run `npm run generate-api-classes` after API changes

## Experimental Svelte Features

The project uses several experimental Svelte features:
- **Async Boundaries**: Enabled for async operations in markup
- **Remote Functions**: Type-safe server communication
- **Tracing**: Built-in observability and tracing
- **Instrumentation**: Custom tracing setup with OpenTelemetry

## Code Quality & Linting

- **ESLint**: With Svelte plugin and TypeScript support
- **Prettier**: With Svelte and Tailwind plugins
- **TypeScript**: Strict mode enabled
- **Svelte-specific rules**: ast-grep configuration in `/rules/`

## Testing

### Unit Tests
- **Frontend**: Vitest with browser testing support
- **Backend**: .NET test framework

### E2E Tests
- **Playwright**: Browser automation testing
- **Configuration**: `/MySvelteApp.Client/playwright.config.ts`

## Observability

- **OpenTelemetry**: Integrated tracing and metrics
- **Grafana**: Visualization dashboard (port 3000)
- **Loki**: Log aggregation (port 3100)
- **Instrumentation**: Custom tracing in `/src/instrumentation.server.js`

## Important Configuration Files

- `svelte.config.js` - SvelteKit with experimental features
- `vite.config.ts` - Vite build configuration
- `tsconfig.json` - TypeScript strict mode
- `tailwind.config.js` - Tailwind CSS setup
- `docker-compose.yml` - Production containers
- `docker-compose.dev.yml` - Development containers