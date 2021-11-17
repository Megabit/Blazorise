---
title: "Steps"
permalink: /docs/components/step/
excerpt: "Learn how to use step components."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/steps/
---

## Basics

Similar to [Tabs]({{ "/docs/components/tab" | relative_url }}), the step component have the same structure and usage.

- `<Steps>` container for Step items
  - `<Items>` container for step items
    - `<Step>` clickable step items
  - `<Content>` container for step panels
    - `<StepPanel>` container for step content

The `Steps` component is container for `Step` items. The `Name` of each step item should match the `Name` of a step panel(if panels are used).

- `<StepsContent>` container for step panels
  - `<StepPanel>` container for step content

The step content container is used to hold step panels. Each content pane also has a unique `Name`, which is targeted by a link in the step-strip.

Most of the time you will only need to use `Steps` component as it is crafted to hold both clickable step items and step content. Only in the advanced scenario where the content will be separated from the step items you will need to use `<StepsContent>` component.

So for a basic steps when we put it all together, we get this:

## Example

```html
<Steps SelectedStep="@selectedStep" SelectedStepChanged="@OnSelectedStepChanged">
    <Items>
        <Step Name="step1">Step 1</Step>
        <Step Name="step2">Step 2</Step>
        <Step Name="step3">Step 3</Step>
        <Step Name="step4">
            <Marker>
                <Icon Name="IconName.Flag" />
            </Marker>
            <Caption>
                Finish
            </Caption>
        </Step>
    </Items>
    <Content>
        <StepPanel Name="step1">
            Content for step 1.
        </StepPanel>
        <StepPanel Name="step2">
            Content for step 2.
        </StepPanel>
        <StepPanel Name="step3">
            Content for step 3.
        </StepPanel>
        <StepPanel Name="step4">
            Content for finish.
        </StepPanel>
    </Content>
</Steps>
@code{
    string selectedStep = "step1";

    private Task OnSelectedStepChanged( string name )
    {
        selectedStep = name;

        return Task.CompletedTask;
    }
}
```

## Functions

| Name                     | Description                                                                                 |
|------------------------- |---------------------------------------------------------------------------------------------|
| SelectStep(string name)  | Sets the active step by the name.                                                           |

## Attributes

### Steps

| Name                  | Type                                                                       | Default          | Description                                                                                           |
|-----------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| SelectedStep          | string                                                                     |                  | Currently selected step name.                                                                         |
| SelectedStepChanged   | event                                                                      |                  | Occurs after the selected step has changed.                                                           |

### Step

| Name                  | Type                                                                       | Default          | Description                                                                                           |
|-----------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| Name                  | string                                                                     | null             | Defines the unique step name.                                                                         |
| Index                 | `int?`                                                                     | null             | Used to override item index.                                                                          |
| Completed             | `bool`                                                                     | false            | Marks the step as completed.                                                                          |
| Color                 | [Colors]({{ "/docs/helpers/colors/#color" | relative_url }})               | `None`   	    | Overrides the step color.                                                                             |
| Clicked               | event                                                                      |                  | Occurs when the button is clicked.                                                                    |
| Marker                | RenderFragment                                                             |                  | Custom render template for the marker(circle) part of the step item.                                  |
| Caption               | RenderFragment                                                             |                  | Custom render template for the text part of the step item.                                            |

### StepsContent

| Name                  | Type                                                                       | Default          | Description                                                                                           |
|-----------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| SelectedPanel         | string                                                                     |                  | Currently selected panel name.                                                                         |
| SelectedPanelChanged  | event                                                                      |                  | Occurs after the selected panel has changed.                                                           |

### StepPanel

| Name                  | Type                                                                       | Default          | Description                                                                                           |
|-----------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| Name                  | string                                                                     | null             | Defines the panel name that must match the corresponding step name.                                     |
