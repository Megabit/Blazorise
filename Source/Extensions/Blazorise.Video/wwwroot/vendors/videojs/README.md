# Video.js HTML distribution

These standalone browser bundles are generated from `@videojs/html@10.0.0-beta.25`:

- `videojs.js` contains the core video preset, UI registrations, and their internal dependencies.
- `hlsjs-video.js` contains the HLS.js media element and HLS.js engine.
- `dash-video.js` contains the DASH media element and dash.js engine.
- `videojs.css` contains the ejected minimal video and audio skin styles and Blazorise compatibility additions.

Each bundle has a matching source map. The ejected skin markup is owned by the Blazorise Video component and loads `videojs.css` from this folder. Package provenance and licensing are recorded in `package.json` and `LICENSE`.

The bundles were produced with esbuild 0.25.9 using browser ESM output, minification, linked source maps, and `--ignore-annotations`. Ignoring package annotations is required because the upstream package marks its UI registration module as side-effect-free even though it registers the custom control elements.

The upstream Google Cast module is included because it is part of the production module graph. Blazorise opts Chromium media elements out of remote playback before upgrade so this module does not download Google's hosted Cast sender SDK at runtime.