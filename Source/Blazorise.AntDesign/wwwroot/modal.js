export function open(element, scrollToTop) {
    if (scrollToTop) {
        const modalBody = element.querySelector('.ant-modal-body');

        if (modalBody) {
            modalBody.scrollTop = 0;
        }
    }
}

export function close(element) {
    // do nothing
}