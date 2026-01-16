import { addClassToBody, removeClassFromBody } from "../Blazorise/utilities.js?v=1.8.9.0";

export function open(element, scrollToTop) {
    addClassToBody("modal-open");

    if (scrollToTop) {
        const modalBody = element.querySelector('.modal-body');

        if (modalBody) {
            modalBody.scrollTop = 0;
        }
    }
}

export function close(element) {
    removeClassFromBody("modal-open");
}