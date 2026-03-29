function getElement(element, elementId) {
    if (element) {
        return element;
    }

    if (elementId) {
        return document.getElementById(elementId);
    }

    return null;
}

function syncThumb(root) {
    const group = root?.querySelector('.ant-segmented-group');
    const thumb = group?.querySelector(':scope > .ant-segmented-thumb');
    const selectedItem = group?.querySelector('.ant-segmented-item-selected');

    if (!group || !thumb) {
        return;
    }

    if (!selectedItem) {
        thumb.style.opacity = '0';
        thumb.style.width = '0px';
        thumb.style.height = '0px';
        thumb.style.transform = 'translate3d(0px, 0px, 0px)';
        return;
    }

    const groupRect = group.getBoundingClientRect();
    const itemRect = selectedItem.getBoundingClientRect();
    const colorClass = Array.from(selectedItem.classList).find(x => x.startsWith('b-ant-segmented-item-'));

    Array.from(thumb.classList)
        .filter(x => x.startsWith('b-ant-segmented-thumb-'))
        .forEach(x => thumb.classList.remove(x));

    if (colorClass) {
        thumb.classList.add(colorClass.replace('b-ant-segmented-item-', 'b-ant-segmented-thumb-'));
    }

    thumb.style.opacity = '1';
    thumb.style.width = `${itemRect.width}px`;
    thumb.style.height = `${itemRect.height}px`;
    thumb.style.transform = `translate3d(${itemRect.left - groupRect.left}px, ${itemRect.top - groupRect.top}px, 0px)`;
}

export function update(element, elementId) {
    const root = getElement(element, elementId);

    if (!root) {
        return;
    }

    requestAnimationFrame(() => syncThumb(root));
}