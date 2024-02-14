export function open(element, scrollToTop) {
    if (scrollToTop) {
        const modalBody = element.querySelector('.modal-card-body');

        if (modalBody) {
            modalBody.scrollTop = 0;
        }
    }
}

export function close(element) {
}