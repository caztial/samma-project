import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import macros from 'unplugin-parcel-macros'
import optimizeLocales from '@react-aria/optimize-locales-plugin'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    macros.vite(), // Must be first!
    {
      ...optimizeLocales.vite({
        locales: ['en-US', 'si-LK']
      }),
      enforce: 'pre'
    },
    react()
  ],
  build: {
    target: ['es2022'],
    cssMinify: 'lightningcss',
    rollupOptions: {
      output: {
        // Bundle all S2 and style-macro generated CSS into a single bundle
        manualChunks(id) {
          if (/macro-(.*)\.css$/.test(id) || /@react-spectrum\/s2\/.*\.css$/.test(id)) {
            return 's2-styles';
          }
        }
      }
    }
  },
  server: {
    port: 5173,
    host: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5001',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})