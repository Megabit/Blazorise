## Build external libraries

At Blazorise, we use several external JS libraries to provide additional functionality. Some of them are already built as valid JS modules, but some are not.

Since those external libraries must be built as valid JS modules to be consumed by Blazorise, we will describe step-by-step instructions for rebuilding external libraries used in Blazorise. 

### quill-resize-module

GitHub: https://github.com/mudoo/quill-resize-module

1. clone the project to your local machine
2. run `yarn install` to install all required dependencies
3. install yarn if you don't have it already, as a global package `npm install -g yarn`
4. open `webpack.config.js` and add

```
output: {
  path: path.resolve(__dirname, 'dist'),
  library: 'QuillResize',
  libraryExport: 'default',
  libraryTarget: 'umd',
  filename: '[name].js',
  clean: true
},
```

5. run `yarn run build`

Now go to the `dist` folder and you will find the `resize.js` file that you copy into the Blazorise project and name it `quill-resize-module.js`.