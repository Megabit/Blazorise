import "./vendors/qr-code-styling.js?v=1.5.2.0";

let _instances = [];

export function initialize(element, elementId, options) {
    const qrCode = new QRCodeStyling(toQrCodeOptions(options));

    qrCode.append(element);

    _instances[elementId] = qrCode;
}

export function update(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance) {
        instance.update(toQrCodeOptions(options));
    }
}

function toQrCodeOptions(options) {
    const typeNumber = 0;
    const moduleCount = typeNumber * 4 + 17;
    const cellSize = options.pixelsPerModule;
    const margin = cellSize * 4;

    const size = (moduleCount * cellSize + margin * 2) + (options.drawQuietZones ? 16 * 2 : 0);

    return {
        width: size,
        height: size,
        type: "canvas",
        data: options.value,
        margin: (options.drawQuietZones ? 16 : 0),
        qrOptions: {
            typeNumber: typeNumber,
            errorCorrectionLevel: options.eccLevel
        },
        dotsOptions: {
            color: options.darkColor,
            type: "square"
        },
        backgroundOptions: {
            color: options.lightColor
        },
        image: options.icon,
        imageOptions: {
            //crossOrigin: "anonymous",
            imageSize: options.iconSizePercentage / 100,
            margin: options.iconBorderWidth
        }
    };
}