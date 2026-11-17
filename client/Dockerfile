FROM node:lts-bookworm-slim

WORKDIR /app

COPY package.json ./
COPY package-lock.json ./

COPY . .

RUN ls -la  
RUN npm install

ENTRYPOINT npm run dev