## Things to be aware of

If you are updating any of the chart packages, please make sure to also update or be aware of the following files accordingly:

- `chartjs-plugin-streaming`: Search `._chart.getDatasetMeta`, and replace with `.chart.getDatasetMeta`
- `luxon.js`: Added a safeguard to expose luxon on globalThis when loaded as a module, ensuring the Chart.js Luxon adapter can access DateTime