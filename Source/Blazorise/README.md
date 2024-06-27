## Build external libraries

At Blazorise, we use several external JS libraries to provide additional functionality. Some of them are already built as valid JS modules, but some are not.

Since those external libraries must be built as valid JS modules to be consumed by Blazorise, we will describe step-by-step instructions for rebuilding external libraries used in Blazorise. 

### Autonumeric

GitHub: https://github.com/autoNumeric/autoNumeric/

1. clone the project to your local machine
2. run `npm install` to install all required dependencies
3. install yarn if you don't have it already, as a global package `npm install -g yarn`
4. open `config\webpack.config.prd.js` and change the `output.globalObject` to `"window"`. (default value is `"this"`)
5. run `npm run build`

Now go to the `dist` folder and you will find the `autoNumeric.min.js` file that you can use in your Blazorise project.