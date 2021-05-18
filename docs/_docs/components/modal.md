---
title: "Modal component"
permalink: /docs/components/modal/
excerpt: "Learn how to use modals."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/modals/
---

**v0.9.3 Breaking changes** `ModalBackdrop` component is removed so you should also remove any usage of it from your project.
`CloseButton` will now automatically close the Modal dialog.
{: .notice--warning}

## Basics

The modal structure is very simple:

- `<Modal>` the main container
  - `<ModalContent>` a horizontally and vertically centered container, in which you can include _any_ content
    - `<ModalHeader>` top part of the modal, usually contains a title and close button
      - `<ModalTitle>` a modal title
      - [`<CloseButton>`](/docs/components/close-button) a simple close button located in the top right corner
    - `<ModalBody>` main part of the modal, holds the input fields, images, etc.
    - `<ModalFooter>` bottom part of the modal, usually contains the action buttons

## Usage

### Basics

Place the modal markup somewhere at root of you component layout.

```html
<Button Clicked="@ShowModal">Show Modal</Button>

<Modal @ref="modalRef">
    <ModalContent Centered="true">
        <ModalHeader>
            <ModalTitle>Employee edit</ModalTitle>
            <CloseButton />
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
            <Button Color="Color.Secondary" Clicked="@HideModal">Close</Button>
            <Button Color="Color.Primary" Clicked="@HideModal">Save Changes</Button>
        </ModalFooter>
    </ModalContent>
</Modal>
```

To work with the modal you must use the reference to the `Modal` component.

```cs
@code{
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

### Closing

If you want to prevent modal from closing you can use `Closing` event.

```html
<Modal @ref="modalRef" Closing="@OnModalClosing">
    ...
</Modal>
```

```cs
@code{
    // reference to the modal component
    private Modal modalRef;

    private void OnModalClosing( CancelEventArgs e )
    {
        // just set Cancel to true to prevent modal from closing
        e.Cancel = true;
    }
}
```

## Functions

| Name         | Description                                                                                 |
|--------------|---------------------------------------------------------------------------------------------|
| Show()       | Open the modal dialog.                                                                      |
| Hide()       | Close the modal dialog.                                                                     |

## Attributes

### Modal

| Name           | Type                                                                   | Default   | Description                                                                                                                    |
|----------------|------------------------------------------------------------------------|-----------|--------------------------------------------------------------------------------------------------------------------------------|
| Visible        | boolean                                                                | false     | Handles the visibility of modal dialog.                                                                                        |
| Closing        | event                                                                  |           | Occurs before the modal is closed and can be used to prevent the modal from closing.                                           |
| ScrollToTop    | boolean                                                                | true      | If true modal will scroll to top when opened.                                                                                  |
| ShowBackdrop   | boolean                                                                | true      | If true the the backdrop will be rendered.                                                                                     |

### ModalContent

| Name           | Type                                                                   | Default   | Description                                                                                                                    |
|----------------|------------------------------------------------------------------------|-----------|--------------------------------------------------------------------------------------------------------------------------------|
| Form           | boolean                                                                | false     | Makes the modal as classic dialog with header, body and footer. Used only by bulma provider! [see Modal card](https://bulma.io/documentation/components/modal#modal-card)                  |
| Centered       | boolean                                                                | false     | Centers the modal vertically.                                                                                                  |
| Size           | [ModalSize]({{ "/docs/helpers/sizes/#modalsize" | relative_url }})     | `Default` | Changes the size of the modal.                                                                                                 |

### ModalBody

| Name           | Type                                                                   | Default   | Description                                                                                                                    |
|----------------|------------------------------------------------------------------------|-----------|--------------------------------------------------------------------------------------------------------------------------------|
| MaxHeight      | int?                                                                   | null      | Sets the maximum height of the modal body (in viewport size unit).                                                             |
