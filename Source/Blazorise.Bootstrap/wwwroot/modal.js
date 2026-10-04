import { addClassToBody, removeClassFromBody } from "../Blazorise/utilities.js?v=__BLAZORISE_VERSION__";
import { adjustDialogDimensionsBeforeShow, closeStackedModal, openStackedModal, registerModalDisconnectCleanup, resetAdjustments, unregisterModalDisconnectCleanup } from "../Blazorise/modal.js?v=__BLAZORISE_VERSION__";

const modalAdjustmentSelectors = {
    fixedContentSelector: ".fixed-top, .fixed-bottom, .is-fixed, .sticky-top",
    stickyContentSelector: ".sticky-top"
};

export function open(element, scrollToTop) {
    registerModalDisconnectCleanup(element, () => closeCore(element));

    openStackedModal(element, {
        beforeOpen: (modalElement) => adjustDialogDimensionsBeforeShow(modalElement, modalAdjustmentSelectors),
        onFirstModalOpen: () => addClassToBody("modal-open"),
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
        onLastModalClose: () => removeClassFromBody("modal-open"),
        afterClose: (modalElement) => resetAdjustments(modalElement, modalAdjustmentSelectors)
    });
}