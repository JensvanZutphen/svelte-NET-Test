# MySvelteApp.Client/Dockerfile

# 1) Build stage
FROM node:24-alpine AS builder
WORKDIR /app

# install all deps including dev dependencies
COPY package*.json ./
RUN npm ci --include=dev

# copy source and build SSR bundle
COPY . .
RUN npm run build       # emits /app/build

# 2) Runtime stage
FROM node:24-alpine AS runner
WORKDIR /app

# copy SSR output
COPY --from=builder /app/build ./build

# copy production deps
COPY --from=builder /app/node_modules ./node_modules

ENV NODE_ENV=production
EXPOSE 3000

# start the Node SSR server
CMD ["node", "build"]
