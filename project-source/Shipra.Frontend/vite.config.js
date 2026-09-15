import { defineConfig, transformWithOxc } from 'vite';
import react from '@vitejs/plugin-react';
import EnvironmentPlugin from 'vite-plugin-environment';

// Custom plugin to parse JSX inside .js files under the src directory using Oxc
const transformJsxInJs = () => ({
  name: 'transform-jsx-in-js',
  enforce: 'pre',
  async transform(code, id) {
    if (!id.match(/src\/.*\.js$/)) {
      return null;
    }
    return await transformWithOxc(code, id, {
      lang: 'jsx',
    });
  },
});

export default defineConfig({
  plugins: [
    react(),
    transformJsxInJs(),
    EnvironmentPlugin('all', { prefix: 'REACT_APP_' })
  ],
  server: {
    port: 3000,
    open: true
  },
  build: {
    outDir: 'build',
    rollupOptions: {
      output: {
        manualChunks(id) {
          if (id.includes('node_modules')) {
            if (id.includes('@mui') || id.includes('@emotion') || id.includes('@material-ui')) {
              return 'vendor-mui';
            }
            if (id.includes('apexcharts') || id.includes('react-apexcharts')) {
              return 'vendor-charts';
            }
            if (id.includes('jspdf') || id.includes('xlsx') || id.includes('html2canvas')) {
              return 'vendor-export';
            }
            if (id.includes('google-map') || id.includes('@react-google-maps')) {
              return 'vendor-maps';
            }
            if (id.includes('react') || id.includes('react-dom') || id.includes('react-router')) {
              return 'vendor-react';
            }
            return 'vendor';
          }
        }
      }
    }
  }
});
