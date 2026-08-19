# Video.js integration

The browser player uses the HTML Web Component build from `@videojs/html@10.0.0-beta.25`.

The production browser code is stored as three standalone bundles under `wwwroot/vendors/videojs`: the core player and UI, HLS.js media, and DASH media. The core bundle is always loaded, while HLS and DASH are loaded only when requested. Matching source maps, package metadata, the ejected minimal-skin stylesheet, and the license are included. The JS interop module loads only these same-origin assets, allowing the extension to work with restrictive Content Security Policies and without internet access.

Remote Playback and AirPlay are handled by Video.js and the browser. The ejected Blazorise skin exposes AirPlay without loading an external sender SDK; all packaged runtime resources remain same-origin and available offline.

Blazorise uses ejected minimal video and audio skins so the existing public control, settings, idle-delay, quality, gesture, and event APIs remain configurable. The owned skin markup is maintained in `Video.razor`; its upstream styles and Blazorise compatibility additions are maintained in `wwwroot/vendors/videojs/videojs.css`.

YouTube and Vimeo media elements are still under development upstream in Video.js v10 and are not included in this integration.