---
title: "Repeater component"
permalink: /docs/components/repeater/
excerpt: "Learn how to use the repeater."
toc: true
toc_label: "Guide"
---

## Basics

The repeater component is a helper component that repeats the child content for each element in a collection. With support for [INotifyCollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.specialized.inotifycollectionchanged).

## Usage

By default a dropdown toggle will open and close a dropdown menu without the need to do it manually. In case you need to control the menu programmatically you have to use the Dropdown reference.

```html
<ul>
    <Repeater Items="@Items" Skip="@Skip" Take="@Take" CollectionChanged="@OnCollectionChanged">
        <li>@context.Name</li>
    </Repeater>
</ul>
```

## Attributes

| Name               | Type                                               | Default      | Description                                                                                              |
|--------------------|----------------------------------------------------|--------------|----------------------------------------------------------------------------------------------------------|
| TItem              | generic item type                                  |              | The item type to render.                                                                                 |
| Items              | IEnumberable\<TItem\>                              | null         | The items to render. When this is `INotifyCollectionChanged` it will hookup collection change listeners. |
| Skip               | long?                                              | null         | The number of items to skip before starting to render                                                    |
| Take               | long?                                              | null         | The number of items to render.                                                                           |
| ChildContent       | RenderFragment\<TItem\>                            |              | The content to render per item.                                                                          |
| CollectionChanged  | EventCallback\<NotifyCollectionChangedEventArgs\>  |              | Occurs when the Items collection changes.                                                                |
