import { addClassToBody, removeClassFromBody } from "../Blazorise/utilities.js?v=2.1.1.0";
import { adjustDialogDimensionsBeforeShow, closeStackedModal, openStackedModal, registerModalDisconnectCleanup, resetAdjustments, unregisterModalDisconnectCleanup } from "../Blazorise/modal.js?v=2.1.1.0";

const modalAdjustmentSelectors = {
    fixedContentSelector: ".fixed-top, .fixed-bottom, .is-fixed, .sticky-top",
    stickyContentSelector: ".sticky-top"
};

export function open(element, scrollToTop) {
    registerModalDisconnectCleanup(element, () => closeCore(element));

    openStackedModal(element, {
        beforeOpen: (modalElement) => adjustDialogDimensionsBeforeShow(modalElement, modalAdjustmentSelectors),
        onFirstModalOpen: () => {
            const originalOverflow = document.body.style.overflow || '';
            document.body.setAttribute('data-original-overflow', originalOverflow);
            document.body.style.overflow = 'hidden';
            addClassToBody("modal-open");
        },
        scrollToTop: scrollToTop,
        bodySelector: ".modal-body"
    });
}

export function close(element) {
    unregisterModalDisconnectCleanup(element);
    closeCore(element);
}

function closeCore(element) {
    closeStackedModal(element, {
        onLastModalClose: () => {
            document.body.style.overflow = document.body.getAttribute('data-original-overflow') || '';
            document.body.removeAttribute('data-original-overflow');
            removeClassFromBody("modal-open");
        },
        afterClose: (modalElement) => resetAdjustments(modalElement, modalAdjustmentSelectors)
    });
}