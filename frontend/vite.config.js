import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import macros from 'unplugin-parcel-macros';
import optimizeLocales from '@react-aria/optimize-locales-plugin';

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  // Load all VITE_* env vars for the current mode so we can use them here
  // (import.meta.env is not available in vite.config.js itself)
  const env = loadEnv(mode, process.cwd(), '');

  // Derive the proxy target (scheme + host + port) from VITE_API_URL.
  // Falls back to http://localhost:5001 if the variable is not set.
  let proxyTarget = 'http://localhost:5001';
  const apiUrl = env.VITE_API_URL;
  if (apiUrl) {
    try {
      proxyTarget = new URL(apiUrl).origin; // e.g. "http://localhost:5001"
    } catch {
      console.warn('[vite] Could not parse VITE_API_URL for proxy target:', apiUrl);
    }
  }

  return {
    plugins: [
      macros.vite(), // Must be first!
      {
        ...optimizeLocales.vite({
          locales: ['en-US', 'si-LK'],
        }),
        enforce: 'pre',
      },
      react(),
    ],

    build: {
      target: ['es2022'],
      cssMinify: 'lightningcss',
      rollupOptions: {
        output: {
          // Bundle all S2 and style-macro generated CSS into a single bundle
          manualChunks(id) {
            if (
              /macro-(.*)\.css$/.test(id) ||
              /@react-spectrum\/s2\/.*\.css$/.test(id)
            ) {
              return 's2-styles';
            }
          },
        },
      },
    },

    server: {
      port: 5173,
      host: true,
      proxy: {
        // All /api/* requests are forwarded to the backend.
        // Change VITE_API_URL in .env.development to point at a different host/port.
        '/api': {
          target: proxyTarget,
          changeOrigin: true,
          secure: false,
        },
      },
    },
  };
});
