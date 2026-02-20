## Things to be aware of

If you are updating any of the chart packages, please make sure to also update or be aware of the following files accordingly:

### `chartjs-plugin-streaming.js`

Build and use from: https://github.com/Megabit/chartjs-plugin-streaming

### `chartjs-plugin-zoom.js`

Build and use from: https://github.com/Megabit/chartjs-plugin-zoom

### `luxon.js`

Added a safeguard to expose luxon on `globalThis` when loaded as a module, ensuring the Chart.js Luxon adapter can access DateTime

Add the following code snippet at the bottom of the `luxon.js` file:

```javascript
"undefined"!=typeof globalThis&&globalThis&&(globalThis.luxon=globalThis.luxon||luxon);
```
