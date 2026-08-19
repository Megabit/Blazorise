# Video.js integration

The browser player and every media provider are pinned to Video.js v10 commit `afb62fe838b41357fd836940594455a6b3156052`, which declares `@videojs/html@10.0.0-beta.28`.

The production browser code is stored as standalone bundles under `wwwroot/vendors/videojs`: the core player and UI plus HLS.js, DASH, YouTube, and Vimeo media providers. The core bundle is always loaded, while the matching provider bundle is loaded only when requested. Matching source maps, package metadata, the ejected minimal-skin stylesheet, and the license are included. The JS interop module loads these bundles from the same origin, allowing the extension to work with restrictive Content Security Policies without JavaScript or CSS CDN references.

YouTube and Vimeo URLs are detected automatically. Their Video.js adapters are served locally, but playback still requires network access to the selected provider's iframe API and media services.

Remote Playback and AirPlay are handled by Video.js and the browser. The ejected Blazorise skin exposes AirPlay without loading an external sender SDK; all packaged runtime resources remain same-origin and available offline.

Blazorise uses Video.js's `minimal-ui` registration entry with ejected minimal video and audio skins so the existing public control, settings, idle-delay, quality, gesture, and event APIs remain configurable. The owned skin markup is maintained in `Video.razor`; its beta.28 base styles and Blazorise compatibility additions are maintained in `wwwroot/vendors/videojs/videojs.css`.