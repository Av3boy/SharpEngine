const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function (app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: 'https://localhost:7215',
      changeOrigin: true,
      secure: false, // allow self-signed HTTPS certs in dev
      logLevel: 'debug',
    })
  );
};
