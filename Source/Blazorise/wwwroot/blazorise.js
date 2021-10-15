if (!window.blazorise) {
    window.blazorise = {};
}

window.blazorise = {
    utils: {
        getRequiredElement: (element, elementId) => {
            if (element)
                return element;

            return document.getElementById(elementId);
        }
    },

    // sets the value to the element property
    setProperty: (element, property, value) => {
        if (element && property) {
            element[property] = value;
        }
    },

    getElementInfo: (element, elementId) => {
        if (!element) {
            element = document.getElementById(elementId);
        }

        if (element) {
            const position = element.getBoundingClientRect();

            return {
                boundingClientRect: {
                    x: position.x,
                    y: position.y,
                    top: position.top,
                    bottom: position.bottom,
                    left: position.left,
                    right: position.right,
                    width: position.width,
                    height: position.height
                },
                offsetTop: element.offsetTop,
                offsetLeft: element.offsetLeft,
                offsetWidth: element.offsetWidth,
                offsetHeight: element.offsetHeight,
                scrollTop: element.scrollTop,
                scrollLeft: element.scrollLeft,
                scrollWidth: element.scrollWidth,
                scrollHeight: element.scrollHeight,
                clientTop: element.clientTop,
                clientLeft: element.clientLeft,
                clientWidth: element.clientWidth,
                clientHeight: element.clientHeight
            };
        }

        return {};
    },

    setTextValue(element, value) {
        element.value = value;
    },

    hasSelectionCapabilities: (element) => {
        const nodeName = element && element.nodeName && element.nodeName.toLowerCase();

        return (
            nodeName &&
            ((nodeName === 'input' &&
                (element.type === 'text' ||
                    element.type === 'search' ||
                    element.type === 'tel' ||
                    element.type === 'url' ||
                    element.type === 'password')) ||
                nodeName === 'textarea' ||
                element.contentEditable === 'true')
        );
    },

    setCaret: (element, caret) => {
        if (window.blazorise.hasSelectionCapabilities(element)) {
            window.requestAnimationFrame(() => {
                element.selectionStart = caret;
                element.selectionEnd = caret;
            });
        }
    },

    getCaret: (element) => {
        return window.blazorise.hasSelectionCapabilities(element)
            ? element.selectionStart :
            -1;
    },

    getSelectedOptions: (elementId) => {
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
    },

    setSelectedOptions: (elementId, values) => {
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
    },

    focus: (element, elementId, scrollToElement) => {
        element = window.blazorise.utils.getRequiredElement(element, elementId);

        if (element) {
            element.focus({
                preventScroll: !scrollToElement
            });
        }
    },
    select: (element, elementId, focus) => {
        if (focus) {
            window.blazorise.focus(element, elementId, true);
        }

        element = window.blazorise.utils.getRequiredElement(element, elementId);

        if (element) {
            element.select();
        }
    },
    theme: {
        addVariable: (name, value) => {
            const themeVariablesElement = document.getElementById("b-theme-variables");

            // make sure that themeVariables element exists and that we don't have the variable already defined
            if (themeVariablesElement && themeVariablesElement.innerHTML) {
                const newVariable = "\n" + name + ": " + value + ";";

                const variableStartIndex = themeVariablesElement.innerHTML.indexOf(name + ":");

                if (variableStartIndex >= 0) {
                    const variableEndIndex = themeVariablesElement.innerHTML.indexOf(";", variableStartIndex);
                    const existingVariable = themeVariablesElement.innerHTML.substr(variableStartIndex, variableEndIndex);

                    const result = themeVariablesElement.innerHTML.replace(existingVariable, newVariable);

                    themeVariablesElement.innerHTML = result;
                }
                else {
                    const innerHTML = themeVariablesElement.innerHTML;
                    const position = innerHTML.lastIndexOf(';');

                    if (position >= 0) {
                        const result = [innerHTML.slice(0, position + 1), newVariable, innerHTML.slice(position + 1)].join('');

                        themeVariablesElement.innerHTML = result;
                    }
                }

                return;
            }

            // The fallback mechanism for custom CSS variables where we don't use theme provider
            // is to apply them to the body element
            document.body.style.setProperty(name, value);
        }
    },

    link: {
        scrollIntoView: (elementId) => {
            var element = document.getElementById(elementId);

            if (element) {
                element.scrollIntoView();
                window.location.hash = elementId;
            }
        }
    },

    table: {
        initializeTableFixedHeader: function (element, elementId) {
            let resizeTimeout = null
            this.resizeThottler = function () {
                if (!resizeTimeout) {
                    resizeTimeout = setTimeout(function () {
                        resizeTimeout = null;
                        resizeHandler(element);
                    }.bind(this), 66);
                }
            }
            function resizeHandler(element) {
                const tableRows = element.querySelectorAll("thead tr");
                if (tableRows !== null && tableRows.length > 1) {
                    let previousRowCellHeight = 0;
                    for (let i = 0; i < tableRows.length; i++) {
                        let currentTh = tableRows[i].querySelectorAll("th");
                        currentTh.forEach(x => x.style.top = `${previousRowCellHeight}px`);
                        previousRowCellHeight += currentTh[0].offsetHeight;
                    }
                }
            }
            resizeHandler(element);
            window.addEventListener("resize", this.resizeThottler, false);
        },
        destroyTableFixedHeader: function (element, elementId) {
            if (typeof this.resizeThottler === "function") {
                window.removeEventListener("resize", this.resizeThottler);
            }
            const tableRows = element.querySelectorAll("thead tr");

            if (tableRows !== null && tableRows.length > 1) {
                for (let i = 0; i < tableRows.length; i++) {
                    let currentTh = tableRows[i].querySelectorAll("th");
                    currentTh.forEach(x => x.style.top = `${0}px`);
                }
            }
        },
        fixedHeaderScrollTableToPixels: function (element, elementId, pixels) {
            if (element !== null && element.parentElement !== null) {
                element.parentElement.scrollTop = pixels;
            }
        },
        fixedHeaderScrollTableToRow: function (element, elementId, row) {
            if (element !== null) {
                let rows = element.querySelectorAll("tr");
                let rowsLength = rows.length;

                if (rowsLength > 0 && row >= 0 && row < rowsLength) {
                    rows[row].scrollIntoView({
                        behavior: "smooth",
                        block: "start"
                    });
                }
            }
        },
        initializeResizable: function (element, elementId, mode) {
            const resizerClass = "b-table-resizer";
            const resizingClass = "b-table-resizing";
            const resizerHeaderMode = 0;
            let cols = null;

            if (element !== null) {
                cols = element.querySelectorAll('thead tr:first-child > th');
            }

            if (cols !== null) {

                const calculateTableActualHeight = function () {
                    let height = 0;
                    if (element !== null) {
                        const tableRows = element.querySelectorAll('tr');

                        tableRows.forEach(x => {
                            let firstCol = x.querySelector('th:first-child,td:first-child');
                            if (firstCol !== null) {
                                height += firstCol.offsetHeight;
                            }
                        });
                    }
                    return height;
                };

                const calculateModeHeight = () => {
                    return mode === resizerHeaderMode
                        ? element !== null
                            ? element.querySelector('tr:first-child > th:first-child').offsetHeight
                            : 0
                        : calculateTableActualHeight();
                };

                let actualHeight = calculateModeHeight();

                const createResizableColumn = function (col) {
                    if (col.querySelector(`.${resizerClass}`) !== null)
                        return;
                    // Add a resizer element to the column
                    const resizer = document.createElement('div');
                    resizer.classList.add(resizerClass);

                    // Set the height
                    resizer.style.height = `${actualHeight}px`;

                    resizer.addEventListener("click", function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                    });

                    let mouseDownDate;
                    let mouseUpDate;

                    col.addEventListener('click', function (e) {
                        let resized = (mouseDownDate !== null && mouseUpDate !== null);
                        if (resized) {
                            let currentDate = new Date();

                            // Checks if mouse down was some ms ago, which means click from resizing
                            let elapsedFromMouseDown = currentDate - mouseDownDate;
                            let clickFromResize = elapsedFromMouseDown > 100;

                            // Checks if mouse up was some ms ago, which either means: 
                            // we clicked from resizing just now or 
                            // did not click from resizing and should handle click normally.
                            let elapsedFromMouseUp = currentDate - mouseUpDate;
                            let clickFromResizeJustNow = elapsedFromMouseUp < 100;

                            if (resized && clickFromResize && clickFromResizeJustNow) {
                                e.preventDefault();
                                e.stopPropagation();
                            }
                            mouseDownDate = null;
                            mouseUpDate = null;
                        }
                    });
                    col.appendChild(resizer);

                    // Track the current position of mouse
                    let x = 0;
                    let w = 0;

                    const mouseDownHandler = function (e) {
                        mouseDownDate = new Date();

                        // Get the current mouse position
                        x = e.clientX;

                        // Calculate the current width of column
                        const styles = window.getComputedStyle(col);
                        w = parseInt(styles.width, 10);

                        // Attach listeners for document's events
                        document.addEventListener('pointermove', mouseMoveHandler);
                        document.addEventListener('pointerup', mouseUpHandler);

                        resizer.classList.add(resizingClass);
                    };

                    const mouseMoveHandler = function (e) {
                        // Determine how far the mouse has been moved
                        const dx = e.clientX - x;

                        resizer.style.height = `${calculateTableActualHeight()}px`;

                        // Update the width of column
                        col.style.width = `${w + dx}px`;
                    };

                    // When user releases the mouse, remove the existing event listeners
                    const mouseUpHandler = function () {
                        mouseUpDate = new Date();

                        resizer.classList.remove(resizingClass);

                        element.querySelectorAll(`.${resizerClass}`).forEach(x => x.style.height = `${calculateModeHeight()}px`);

                        document.removeEventListener('pointermove', mouseMoveHandler);
                        document.removeEventListener('pointerup', mouseUpHandler);
                    };

                    resizer.addEventListener('pointerdown', mouseDownHandler);
                };


                [].forEach.call(cols, function (col) {
                    createResizableColumn(col);
                });
            }
        },
        destroyResizable: function (element, elementId) {
            if (element !== null) {
                element.querySelectorAll('.b-table-resizer').forEach(x => x.remove());
            }
        }
    }
};