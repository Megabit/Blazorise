# Video.js HTML distribution

All browser bundles and base skin styles are generated from Video.js v10 commit `afb62fe838b41357fd836940594455a6b3156052`, which declares `@videojs/html@10.0.0-beta.28`:

- `videojs.js` contains the `minimal-ui` registrations used by the ejected Blazorise skin and their internal dependencies.
- `hlsjs-video.js` contains the HLS.js media element and HLS.js engine.
- `dash-video.js` contains the DASH media element and dash.js engine.
- `youtube-video.js` contains the YouTube media element and adapter.
- `vimeo-video.js` contains the Vimeo media element, adapter, and Vimeo Player SDK.
- `videojs.css` contains the beta.28 minimal video and audio skin styles and Blazorise compatibility additions.

Each JavaScript bundle has a matching source map. The ejected skin markup is owned by the Blazorise Video component and loads `videojs.css` from this folder. The upstream package snapshot and license are recorded in `package.json` and `LICENSE`.

The JavaScript bundles were produced with esbuild 0.28.1 using browser ESM output, an ES2022 target, minification, linked source maps, and `--ignore-annotations`. Ignoring package annotations is required because the upstream package marks registration modules as side-effect-free even though they register custom elements. Each entry is self-contained, so the core remains one file and no shared runtime chunks are required.

All bundle and skin files are served locally. YouTube and Vimeo playback still connects to the selected provider's iframe API and media services.