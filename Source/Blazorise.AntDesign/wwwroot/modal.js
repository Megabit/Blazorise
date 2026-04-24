import { scrollModalBodyToTop } from "../Blazorise/modal.js?v=2.1.1.0";

export function open(element, scrollToTop) {
    scrollModalBodyToTop(element, scrollToTop, ".ant-modal-body");
}

export function close(element) {
    // do nothing
}