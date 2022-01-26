export function parseFunction(str) {
    try {
        let fn_body_idx = str.indexOf('{'),
            fn_body = str.substring(fn_body_idx + 1, str.lastIndexOf('}')),
            fn_declare = str.substring(0, fn_body_idx),
            fn_params = fn_declare.substring(fn_declare.indexOf('(') + 1, fn_declare.lastIndexOf(')')),
            args = fn_params.split(',');

        args.push(fn_body);

        function Fn() {
            return Function.apply(this, args);
        }

        Fn.prototype = Function.prototype;

        return new Fn();
    }
    catch (error) {
        return null;
    }
}

export function deepClone(value) {
    return JSON.parse(JSON.stringify(value));
}