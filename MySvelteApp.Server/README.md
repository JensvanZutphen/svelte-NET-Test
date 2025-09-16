# Authentication Implementation

This document describes the authentication system implemented for the Svelte.NET application.

## Features

1. User registration and login with secure password hashing
2. Entity Framework for data persistence (using in-memory database for simplicity)
3. Global authorization policy requiring authentication for all endpoints by default
4. Opt-out mechanism for public endpoints using `[AllowAnonymous]`

## Models

### Auth Models
- `User`: Represents a registered user with username, email, and hashed password
- `LoginModel`: DTO for login requests containing username and password
- `RegisterModel`: DTO for registration requests containing username, email, and password

## Controllers

### AuthController
- `/Auth/register` - Register a new user
- `/Auth/login` - Authenticate a user

### Protected Controllers
- All controllers require authentication by default
- Public endpoints are explicitly marked with `[AllowAnonymous]`

## Implementation Details

1. Passwords are securely hashed using HMACSHA512
2. Entity Framework with in-memory database for persistence
3. Cookie-based authentication
4. Global authorization policy requiring authenticated users for all endpoints
5. CORS configured for client-server communication

## Usage

1. Register a new user by POSTing to `/Auth/register` with a username, email, and password
2. Login by POSTing to `/Auth/login` with username and password
3. Once authenticated, all other endpoints will be accessible
4. Public endpoints (like weather and pokemon) are accessible without authentication 