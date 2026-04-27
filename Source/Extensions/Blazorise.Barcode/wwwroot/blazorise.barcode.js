import "./vendors/bwip-js-min.js?v=2.1.1.0";

const barcodeTypeMap = {
    aztec: "azteccode",
    code128: "code128",
    code39: "code39",
    code93: "code93",
    codabar: "rationalizedCodabar",
    codablockF: "codablockf",
    dataMatrix: "datamatrix",
    dotCode: "dotcode",
    ean13: "ean13",
    ean8: "ean8",
    hanXin: "hanxin",
    interleaved2Of5: "interleaved2of5",
    itf14: "itf14",
    mailmark: "mailmark",
    maxiCode: "maxicode",
    microPdf417: "micropdf417",
    msi: "msi",
    pdf417: "pdf417",
    pharmacode: "pharmacode",
    qrCode: "qrcode",
    upcA: "upca",
    upcE: "upce"
};

const renderModeMap = {
    canvas: "canvas",
    svg: "svg"
};

const valueAlignmentMap = {
    center: "center",
    end: "right",
    start: "left"
};

const rotationMap = {
    inverted: "I",
    left: "L",
    none: "N",
    right: "R"
};

let colorParser;

export function initialize(element, elementId, options) {
    render(element, options);
}

export function update(element, elementId, options) {
    render(element, options);
}

function render(element, options) {
    if (!element) {
        return;
    }

    element.innerHTML = "";

    if (!options || !options.value) {
        return;
    }

    const renderOptions = toBarcodeOptions(options);

    try {
        if (getMappedValue(renderModeMap, options.renderMode) === "svg") {
            element.innerHTML = getBarcodeProvider().toSVG(renderOptions);
        } else {
            const canvas = document.createElement("canvas");
            element.appendChild(canvas);
            getBarcodeProvider().toCanvas(canvas, renderOptions);
        }
    } catch (error) {
        showError(element, error);
    }
}

function getBarcodeProvider() {
    if (!globalThis.bwipjs) {
        throw new Error("Barcode provider is not loaded.");
    }

    return globalThis.bwipjs;
}

function toBarcodeOptions(options) {
    const providerOptions = options.providerOptions || {};
    const barcodeOptions = {
        bcid: getMappedValue(barcodeTypeMap, options.type),
        text: options.value,
        scale: options.scale || 2,
        rotate: getMappedValue(rotationMap, options.rotation),
        includetext: options.showValue === true,
        textxalign: getMappedValue(valueAlignmentMap, options.valueAlignment),
        barcolor: normalizeColor(options.foregroundColor),
        backgroundcolor: normalizeColor(options.backgroundColor)
    };

    if (options.width !== null && options.width !== undefined) {
        barcodeOptions.width = options.width;
    }

    if (options.height !== null && options.height !== undefined) {
        barcodeOptions.height = options.height;
    }

    if (options.paddingTop !== null && options.paddingTop !== undefined) {
        barcodeOptions.paddingtop = options.paddingTop;
    }

    if (options.paddingRight !== null && options.paddingRight !== undefined) {
        barcodeOptions.paddingright = options.paddingRight;
    }

    if (options.paddingBottom !== null && options.paddingBottom !== undefined) {
        barcodeOptions.paddingbottom = options.paddingBottom;
    }

    if (options.paddingLeft !== null && options.paddingLeft !== undefined) {
        barcodeOptions.paddingleft = options.paddingLeft;
    }

    return { ...barcodeOptions, ...providerOptions };
}

function getMappedValue(map, value) {
    if (!value) {
        return undefined;
    }

    const key = value.charAt(0).toLowerCase() + value.slice(1);

    return map[key] || key;
}

function normalizeColor(value) {
    if (!value) {
        return undefined;
    }

    const color = value.trim();

    if (color.toLowerCase() === "transparent") {
        return undefined;
    }

    const hexColor = color.charAt(0) === "#"
        ? color.substring(1)
        : color;

    if (/^[0-9a-fA-F]{6}$/.test(hexColor)) {
        return hexColor;
    }

    if (/^[0-9a-fA-F]{3}$/.test(hexColor)) {
        return hexColor.split("").map(value => value + value).join("");
    }

    if (globalThis.CSS && !globalThis.CSS.supports("color", color)) {
        return hexColor;
    }

    if (!colorParser) {
        colorParser = document.createElement("canvas").getContext("2d");
    }
    colorParser.fillStyle = "#000000";
    colorParser.fillStyle = color;

    const parsedColor = colorParser.fillStyle;

    if (/^#[0-9a-fA-F]{6}$/.test(parsedColor)) {
        return parsedColor.substring(1);
    }

    const rgb = parsedColor.match(/^rgba?\((\d+),\s*(\d+),\s*(\d+)/);

    if (rgb) {
        return [rgb[1], rgb[2], rgb[3]]
            .map(value => Number(value).toString(16).padStart(2, "0"))
            .join("");
    }

    return hexColor;
}

function showError(element, error) {
    const errorElement = document.createElement("span");
    errorElement.setAttribute("role", "alert");
    errorElement.textContent = error && error.message ? error.message : String(error);
    element.appendChild(errorElement);
}