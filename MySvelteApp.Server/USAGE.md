# Authentication API Examples

## Register a new user

```bash
curl -X POST http://localhost:5000/Auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "securepassword"
  }'
```

## Login

```bash
curl -X POST http://localhost:5000/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "securepassword"
  }'
```

## Access a protected endpoint (after login)

```bash
curl http://localhost:5000/TestAuth
```

## Access a public endpoint (no authentication required)

```bash
curl http://localhost:5000/WeatherForecast
```

```bash
curl http://localhost:5000/RandomPokemon
``` 