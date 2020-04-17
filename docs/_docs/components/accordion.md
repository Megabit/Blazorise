---
title: "Accordion component"
permalink: /docs/components/accordion/
excerpt: "Learn how to use accordion and collapse components."
toc: true
toc_label: "Guide"
---

A content area which can be collapsed and expanded.

## Basics

The `Accordion` and `Collapse` structure is very simple:

- `<Accordion>` the main container
  - `<Collapse>` defines an collapsible element - controls the `Visible` state.
    - `<CollapseHeader>` the header that will always be shown - this is where you would put a toggle element (such as a `Button`).
    - `<CollapseBody>` the main content that will be toggled.

## Usage

Here is an example of how to use the `Accordion` with the `Collapse` components.

```html
<Accordion>
    <Collapse Visible="@collapse1Visible">
        <CollapseHeader>
            <Heading Size="HeadingSize.Is5">
                <Button Clicked="@(()=>collapse1Visible = !collapse1Visible)">Switch 1</Button>
            </Heading>
        </CollapseHeader>
        <CollapseBody>
            Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
        </CollapseBody>
    </Collapse>
    <Collapse Visible="@collapse2Visible">
        <CollapseHeader>
            <Heading Size="HeadingSize.Is5">
                <Button Clicked="@(()=>collapse2Visible = !collapse2Visible)">Switch 2</Button>
            </Heading>
        </CollapseHeader>
        <CollapseBody>
            Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
        </CollapseBody>
    </Collapse>
    <Collapse Visible="@collapse3Visible">
        <CollapseHeader>
            <Heading Size="HeadingSize.Is5">
                <Button Clicked="@(()=>collapse3Visible = !collapse3Visible)">Switch 3</Button>
            </Heading>
        </CollapseHeader>
        <CollapseBody>
            Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
        </CollapseBody>
    </Collapse>
</Accordion>
@code{
    bool collapse1Visible = true;
    bool collapse2Visible = false;
    bool collapse3Visible = false;
}
```

## Attributes

### Collapse

| Name         | Type                                                         | Default          | Description                                                                                 |
|--------------|--------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Visible      | boolean                                                      | false            | Sets the collapse visibility.                                                               |