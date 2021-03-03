---
title: "Button component"
permalink: /docs/components/button/
excerpt: "Learn how to use buttons."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/buttons/
---

The button is an essential element of any design. It's meant to look and behave as an interactive element of your page.

## Basic button

To create a basic button you need to use a Button component.

```html
<Button>Click me</Button>
```

<iframe src="/examples/buttons/basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### How to use

To use button you just handle a button `Clicked` event.

```html
<Button Clicked="@OnButtonClicked">Click me</Button>
```

```cs
@code{
    void OnButtonClicked()
    {
        Console.WriteLine( "Hello world!" );
    }
}
```

### Colors

To define button color use a `Color` attribute.

```html
<Button Color="Color.Primary">Primary</Button>
<Button Color="Color.Secondary">Secondary</Button>
<Button Color="Color.Success">Success</Button>
<Button Color="Color.Warning">Warning</Button>
<Button Color="Color.Danger">Danger</Button>
<Button Color="Color.Info">Info</Button>
<Button Color="Color.Light">Light</Button>
<Button Color="Color.Dark">Dark</Button>
<Button>None</Button>
```

<iframe src="/examples/buttons/colors/" frameborder="0" scrolling="no" style="width:100%;height:90px;"></iframe>

**Note:** To find the list of supported colors please look at the [colors]({{ "/docs/helpers/colors/" | relative_url }}) page.
{: .notice--info}

### Outline

To define button color use a `Color` attribute.

```html
<Button Color="Color.Primary" Outline="true">Primary</Button>
<Button Color="Color.Secondary" Outline="true">Secondary</Button>
<Button Color="Color.Warning" Outline="true">Warning</Button>
<Button Color="Color.Danger" Outline="true">Danger</Button>
```

<iframe src="/examples/buttons/outlined/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Block

```html
<Button Color="Color.Primary" Block="true">Blocked primary</Button>
<Button Color="Color.Secondary" Block="true">Blocked secondary</Button>
```

<iframe src="/examples/buttons/block/" frameborder="0" scrolling="no" style="width:100%;height:95px;"></iframe>

### Active

```html
<Button Active="true">Primary</Button>
<Button Active="true">Secondary</Button>
```

<iframe src="/examples/buttons/active/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Disabled

```html
<Button Disabled="true">Primary</Button>
<Button Disabled="true">Secondary</Button>
```

<iframe src="/examples/buttons/disabled/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Loading

Use `Loading` attribute to add spinners within buttons to indicate an action is currently processing or taking place. Use `<LoadingTemplate>` to add a custom loading template

```html
<Button Loading="true">Primary</Button>
<Button Loading="true">
    <LoadingTemplate>Custom loading template</LoadingTemplate>
    <ChildContent>Primary</ChildContent>
</Button>
```

<iframe src="/examples/buttons/loading/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Button group

If you want to group buttons together on a single line, use the `Buttons` tag.

```html
<Buttons>
    <Button Color="Color.Secondary">LEFT</Button>
    <Button Color="Color.Secondary">CENTER</Button>
    <Button Color="Color.Secondary">RIGHT</Button>
</Buttons>
```

<iframe src="/examples/buttons/buttongroup/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Link Button

By default, `Button` works with `<button>` element, but you can also create an `<a>` element that will still appear as regular button.

```html
<Button Color="Color.Primary" Type="ButtonType.Link" To="#">Primary link</Button>
<Button Color="Color.Secondary" Type="ButtonType.Link" To="#" Target="Target.Blank">Secondary link</Button>
```

<iframe src="/examples/buttons/linkbutton/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Toolbar

To attach buttons together use a Toolbar role.

```html
<Buttons Role="ButtonsRole.Toolbar">
    <Buttons Margin="Margin.Is2.FromRight">
        <Button Color="Color.Primary">Primary</Button>
        <Button Color="Color.Secondary">Secondary</Button>
        <Button Color="Color.Info">Info</Button>
    </Buttons>
    <Buttons>
        <Button Color="Color.Danger">Danger</Button>
        <Button Color="Color.Warning">Warning</Button>
    </Buttons>
    <Buttons Margin="Margin.Is2.OnX">
        <Button Color="Color.Success">Success</Button>
    </Buttons>
</Buttons>
```

<iframe src="/examples/buttons/buttontoolbar/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Special cases

### Submit button

When using a submit button inside of `<Form>` element the browser will automatically try to post the page. This is the default browser behavior. Because of this a new attribute is introduced to the `<Button>` element, called `PreventDefaultOnSubmit`. Basically it prevents a default browser behavior when clicking the submit button. So instead of posting the page it will stop it and just call the `Clicked` event handler. Pressing the `Enter` key will still work just as it's supposed to do.

```html
<Form>
    <Field Horizontal="true">
        <FieldLabel ColumnSize="ColumnSize.Is2">Name</FieldLabel>
        <FieldBody ColumnSize="ColumnSize.Is10">
            <TextEdit Placeholder="Some text value..." />
        </FieldBody>
    </Field>
    <Button Type="ButtonType.Submit" PreventDefaultOnSubmit="true">Submit</Button>
</Form>
```

## Attributes

| Name                      | Type                                                                    | Default  			| Description                                                                  |
|---------------------------|-------------------------------------------------------------------------|---------------------|------------------------------------------------------------------------------|
| Color                     | [Color]({{ "/docs/helpers/colors/#color" | relative_url }})             | `None`   			| Component visual or contextual style variants                                |
| Size                      | [Size]({{ "/docs/helpers/sizes/#size" | relative_url }})                | `None`   			| Button size variations.                                                      |
| Type                      | [ButtonType]({{ "/docs/helpers/enums/#buttontype" | relative_url }})    | `Button` 			| Defines the button type.                                                     |
| Clicked                   | event                                                                   |          			| Occurs when the button is clicked.                                           |
| Outline                   | boolean                                                                 | false    			| Outlined button.                                                             |
| Disabled                  | boolean                                                                 | false    			| Makes button look inactive.                                                  |
| Active                    | boolean                                                                 | false    			| Makes the button to appear as pressed.                                       |
| Block                     | boolean                                                                 | false    			| Makes the button to span the full width of a parent.                         |
| Loading                   | boolean                                                                 | false    			| Shows the loading spinner.                                                   |
| LoadingTemplate			| RenderFragment														  |`Loading spinner`	| Changes the default loading spinner to custom content.                       |
| Command                   | ICommand                                                                | null     			| Command to be executed when clicked on a button.                             |
| CommandParameter          | object                                                                  | null     			| Reflects the parameter to pass to the CommandProperty upon execution.        |
| PreventDefaultOnSubmit    | boolean                                                                 | false    			| Prevents the button from submitting the form.                                |