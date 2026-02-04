import './vendors/split.js?v=1.8.10.0';

import { getRequiredElement } from "../Blazorise/utilities.js?v=1.8.10.0";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.Splitter/blazorise.splitter.css?v=1.8.10.0\" />");

const _instances = [];

/**
 * Configuration options for Split
 * @typedef {Object} SplitOptions
 * @property {number[] | undefined} sizes - Initial sizes of each element in percents or CSS values.
 * @property {number | number[] | undefined} minSize - Minimum size of each element.
 * @property {number | number[] | undefined} maxSize - Maximum size of each element.
 * @property {boolean | undefined} expandToMin
 * @property {number | undefined} gutterSize - Gutter size in pixels.
 * @property {'center' | 'start' | 'end' | undefined} gutterAlign -
 * @property {number | number[] | undefined} snapOffset - Snap to minimum size offset in pixels.
 * @property {number | undefined} dragInterval -
 * @property {'horizontal' | 'vertical' | undefined} direction - Direction to split
 * @property {string | undefined} cursor - Cursor to display while dragging.
 * /
 /**
 * Creates a new Split instance
 * @param  {HTMLElement} elements The elements to split
 * @param {SplitOptions} splitterOptions Configuration options
 * @return A split instance
 */
export function initializeSplitter(element, elementId, elements, splitterOptions, splitterGutterOptions) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const parsedSplitterOptions = parseOptions(splitterOptions);

    if (splitterGutterOptions && splitterGutterOptions.backgroundImage) {
        parsedSplitterOptions.gutterStyle = (dimension, gutterSize) => ({
            backgroundImage: `url(${splitterGutterOptions.backgroundImage})`,
            [dimension]: `${gutterSize}px`
        });
    }

    parsedSplitterOptions.gutter = (index, direction) => {
        const gutterElement = document.createElement('div');
        gutterElement.className = `splitter-gutter splitter-gutter-${direction}`;
        return gutterElement;
    };

    const split = Split(elements, parsedSplitterOptions);

    const instance = {
        element: element,
        elementId: elementId,
        split: split
    };

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance && instance.split) {
        if (instance.split) {
            try {
                instance.split.destroy();
            } catch (e) {
                console.error(e);
            }
        }

        delete instances[elementId];
    }
}

/**
 * Parses all the options and prepares them to be input into the split.js library
 * @param {SplitOptions} options Configuration options
 * @return {SplitOptions} Parsed options
 */
function parseOptions(options) {
    return {
        ...options,
        sizes: parseNumberOrArray(options.sizes),
        minSize: parseNumberOrArray(options.minSize),
        maxSize: parseNumberOrArray(options.maxSize),
        gutterSize: parseNumber(options.gutterSize),
        snapOffset: parseNumber(options.snapOffset),
        dragInterval: parseNumber(options.dragInterval)
    };
}

/**
 * Parses the value of an option that is a number or number array
 * @param {number|number[]|string|undefined} value Option that is a number or array of numbers
 * @return {number|number[]|undefined} Parsed option
 */
function parseNumberOrArray(value) {
    if (Array.isArray(value)) {
        const parsedValues = [];
        for (const element of value) {
            parsedValues.push(parseNumber(element));
        }
        return parsedValues;
    }

    return parseNumber(value);
}

/**
 * Parses the value of an option that is a number
 * @param {number|string|undefined} value Number option
 * @return {number|undefined} Parsed option
 */
function parseNumber(value) {
    if (typeof value === 'string' || value instanceof String) {
        if (value === 'Infinity')
            return Number.POSITIVE_INFINITY;
        else if (value === '-Infinity')
            return Number.NEGATIVE_INFINITY;
        else if (value === 'NaN')
            return Number.NaN;
        else
            return undefined;
    }

    return value;
}