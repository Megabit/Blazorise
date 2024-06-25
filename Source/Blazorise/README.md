## Build external libraries

External libraries must be build as a valid JS module to be consumed by Blazorise.

In this section we will describe how to build external libraries that are used in Blazorise, step by step. 

### Autonumeric

GitHub: https://github.com/autoNumeric/autoNumeric/

1. clone the project to your local machine
2. run `npm install` to install all required dependencies
3. install yarn if you don't have it already, as a global package `npm install -g yarn`
4. open `config\webpack.config.prd.js` and change the `output.globalObject` to `"window"`. (default value is `"this"`)
5. run `npm run build`

Now go to the `dist` folder and you will find the `autoNumeric.min.js` file that you can use in your Blazorise project.