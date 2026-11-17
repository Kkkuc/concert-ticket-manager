import {fileURLToPath, URL} from 'node:url'
import react from '@vitejs/plugin-react'
import {defineConfig} from 'vite'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        host: true, 
        port: 5000,
        watch: {
            usePolling: true,
            interval: 100,
        },
        proxy: {
            '/api': {
                target: 'http://backend:3000',
                changeOrigin: true,
            },
            '/auth': {
                target: 'http://backend:3000',
                changeOrigin: true,
            },
        },
    },
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
})
