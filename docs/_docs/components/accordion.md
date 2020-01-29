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
                <Button Clicked="@(()=>collapse1Visible = !collapse1Visible)">Switch 1</Button>
            </Heading>
        </CardHeader>
        <Collapse Visible="@collapse1Visible">
            <CardBody>
                Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
            </CardBody>
        </Collapse>
    </Card>
    <Card>
        <CardHeader>
            <Heading Size="HeadingSize.Is5">
                <Button Clicked="@(()=>collapse2Visible = !collapse2Visible)">Switch 2</Button>
            </Heading>
        </CardHeader>
        <Collapse Visible="@collapse2Visible">
            <CardBody>
                Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
            </CardBody>
        </Collapse>
    </Card>
    <Card>
        <CardHeader>
            <Heading Size="HeadingSize.Is5">
                <Button Clicked="@(()=>collapse3Visible = !collapse3Visible)">Switch 3</Button>
            </Heading>
        </CardHeader>
        <Collapse Visible="@collapse3Visible">
            <CardBody>
                Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
            </CardBody>
        </Collapse>
    </Card>
</Accordion>
@code{
    bool collapse1Visible = true;
    bool collapse2Visible = false;
    bool collapse3Visible = false;
}
```
