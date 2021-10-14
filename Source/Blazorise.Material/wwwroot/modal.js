import { addClassToBody, removeClassFromBody } from "/_content/Blazorise/base.js";

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