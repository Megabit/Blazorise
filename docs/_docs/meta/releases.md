---
title: "Releases"
permalink: /docs/releases/
excerpt: "List of Blazorise versions so far."
toc: true
toc_label: "Version history"
---

## 0.3.0 (unreleased)

### Breaking Changes

  - To allow better flexibility when defining horizontal fields a new tag `FieldBody` is introduced. Horizontal fields are now defined like this:

  ```html
  <Field IsHorizontal="true">
      <FieldLabel ColumnSize="ColumnSize.Is2">Name</FieldLabel>
      <FieldBody ColumnSize="ColumnSize.Is10">
          <TextEdit Placeholder="Some text value..." />
      </FieldBody>
  </Field>
  ```

### Enhancements

- New Breadcrumb components
- Ability to add custom rules for Margin, Padding and ColumnSize
- Added ModalTitle component, Title argument on ModalHeader is now **obsolete**
- Better implementation of Bulma css framework
- Snackbar moved from Blazorise to the Extensions folder

### Bug Fixes

- Paragraph classname was "pagination"

## 0.2.2

### Enhancements

- TabContent renamed to TabsContent
- Added text color to Heading

### Bug Fixes

- BarLink tag is null

## 0.2.1

### Enhancements

- Renamed breakpoints on fluent builders for column size and margin/padding
- Added Screenreader visibility to labels
- Added comments to enums
- Smaller refactoring and bug fixes

### Bug Fixes

- Fixed active links on SidebarLink and BarLink

## 0.2.0

### Enhancements

- Refactored all components to allow a custom implementations in providers while still having a component with the same name in the base project