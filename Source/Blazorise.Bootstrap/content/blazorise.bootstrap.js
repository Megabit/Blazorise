if (!window.blazoriseBootstrap) {
    window.blazoriseBootstrap = {};
}

window.blazoriseBootstrap = {
    //activateDatePicker: (elementId, formatSubmit) => {
    //    const element = $(`#${elementId}`);

    //    element.datepicker({
    //        uiLibrary: 'bootstrap4',
    //        format: 'yyyy-mm-dd',
    //        showOnFocus: true,
    //        showRightIcon: true,
    //        select: function (e, type) {
    //            // trigger onchange event on the DateEdit component
    //            mutateDOMChange(elementId);
    //        }
    //    });
    //    return true;
    //}
};

function mutateDOMChange(id) {
    el = document.getElementById(id);
    ev = document.createEvent('Event');
    ev.initEvent('change', true, false);
    el.dispatchEvent(ev);
}