---
title: "Accordion component"
permalink: /docs/components/accordion/
excerpt: "Learn how to use accordion and collapse components."
toc: true
toc_label: "Guide"
---

## Accordion

Using the `Card` component, you can extend the default collapse behavior to create an accordion.

```html
<Accordion>
    <Card>
        <CardHeader>
            <Heading Size="HeadingSize.Is5">
                <SimpleButton Clicked="@(()=>isOpen1 = !isOpen1)">Switch 1</SimpleButton>
            </Heading>
        </CardHeader>
        <Collapse IsOpen="@isOpen1">
            <CardBody>
                Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
            </CardBody>
        </Collapse>
    </Card>
    <Card>
        <CardHeader>
            <Heading Size="HeadingSize.Is5">
                <SimpleButton Clicked="@(()=>isOpen2 = !isOpen2)">Switch 2</SimpleButton>
            </Heading>
        </CardHeader>
        <Collapse IsOpen="@isOpen2">
            <CardBody>
                Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
            </CardBody>
        </Collapse>
    </Card>
    <Card>
        <CardHeader>
            <Heading Size="HeadingSize.Is5">
                <SimpleButton Clicked="@(()=>isOpen3 = !isOpen3)">Switch 3</SimpleButton>
            </Heading>
        </CardHeader>
        <Collapse IsOpen="@isOpen3">
            <CardBody>
                Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
            </CardBody>
        </Collapse>
    </Card>
</Accordion>
@code{
    bool isOpen1 = true;
    bool isOpen2 = false;
    bool isOpen3 = false;
}
```
