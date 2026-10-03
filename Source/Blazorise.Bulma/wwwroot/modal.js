import { scrollModalBodyToTop } from "../Blazorise/modal.js?v=__BLAZORISE_VERSION__";

export function open(element, scrollToTop) {
    scrollModalBodyToTop(element, scrollToTop, ".modal-card-body");
}

export function close(element) {
}