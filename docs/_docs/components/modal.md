---
title: "Modal component"
permalink: /docs/components/modal/
excerpt: "Learn how to use modals."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/modals/
---

## Basics

The modal structure is very simple:

- `<Modal>` the main container
  - `<ModalBackdrop>` a transparent overlay that can act as a click target to close the modal
  - `<ModalContent>` a horizontally and vertically centered container, in which you can include _any_ content
    - `<ModalHeader>` top part of the modal, usually contains a title and close button
      - `<ModalTitle>` a modal title
      - `<CloseButton>` a simple close button located in the top right corner
    - `<ModalBody>` main part of the modal, holds the input fields, images, etc.
    - `<ModalFooter>` bottom part of the modal, usually contains the action buttons


### Usage

Place the modal markup somewhere at root of you component layout.

```html
<SimpleButton Clicked="@ShowModal">Show Modal</SimpleButton>

<Modal ref="modalRef">
    <ModalBackdrop />
    <ModalContent IsCentered="true">
        <ModalHeader>
            <ModalTitle>Employee edit</ModalTitle>
            <CloseButton Clicked="@HideModal" />
        </ModalHeader>
        <ModalBody>
            <Field>
                <FieldLabel>Name</FieldLabel>
                <TextEdit Placeholder="Enter name..." />
            </Field>
            <Field>
                <FieldLabel>Surname</FieldLabel>
                <TextEdit Placeholder="Enter surname..." />
            </Field>
        </ModalBody>
        <ModalFooter>
            <SimpleButton Color="Color.Secondary" Clicked="@HideModal">Close</SimpleButton>
            <SimpleButton Color="Color.Primary" Clicked="@HideModal">Save Changes</SimpleButton>
        </ModalFooter>
    </ModalContent>
</Modal>
```

To work with the modal you must use the reference to the `Modal` component.

```cs
@functions{
    // reference to the modal component
    private Modal modalRef;

    private void ShowModal()
    {
        modalRef.Show();
    }

    private void HideModal()
    {
        modalRef.Hide();
    }
}
```
