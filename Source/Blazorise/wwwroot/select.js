export function getSelectedOptions(elementId) {
    const element = document.getElementById(elementId);
    const len = element.options.length;
    var opts = [], opt;

    for (var i = 0; i < len; i++) {
        opt = element.options[i];

        if (opt.selected) {
            opts.push(opt.value);
        }
    }

    return opts;
}

export function setSelectedOptions(elementId, values) {
    const element = document.getElementById(elementId);

    if (element && element.options) {
        const len = element.options.length;

        for (var i = 0; i < len; i++) {
            const opt = element.options[i];

            if (values && values.find(x => x !== null && x.toString() === opt.value)) {
                opt.selected = true;
            } else {
                opt.selected = false;
            }
        }
    }
}