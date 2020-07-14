---
title: "File component"
permalink: /docs/components/file/
excerpt: "Documentation and examples for file component."
toc: true
toc_label: "Guide"
---

Customized, cross-browser consistent, file input control that supports single file and multiple files upload.

## Single file (default)

On single file mode, when file is selected or user does not cancel Browse dialog, `Changed` event will be raised. The return value will be a `FileChangedEventArgs` that will contain only one item in the `Files` property.

```html
<FileEdit Changed="@OnChanged" />
```

## Multiple files

Multiple file uploading is supported by enabling `Multiple` attribute to component. In this case `Files` property of `FileChangedEventArgs` can contain multiple files.

```html
<FileEdit Changed="@OnChanged" Multiple="true" />
```

## Limiting to certain file types

You can limit the file types by setting the `Filter` attribute to a string containing the allowed file type(s). To specify more than one type, separate the values with a comma.

```html
<!-- Accept all image formats by IANA media type wildcard-->
<FileEdit Filter="image/*" />

<!-- Accept specific image formats by IANA type -->
<FileEdit Filter="image/jpeg, image/png, image/gif" />

<!-- Accept specific image formats by extension -->
<FileEdit Filter=".jpg, .png, .gif" />
```

To accept any file type, leave `Filter` as null (default).

**Note:** Not all browsers support or respect the accept attribute on file inputs.
{: .notice--info}

## Events

### Changed

This is the main event that will be called every time a user selects a single or multiple files. Depending on the mode in which the `FileEdit` currently operates. In all cases the event argument is the same. Only difference is that `Files` array will contain single or multiple items.

### Written

This event will be called on every buffer of data that has being written to the destination stream. It is directly related to the `MaxMessageSize` attribute found on `FileEdit` component and will contain the information about currently processed file, it's offset and data array.

### Progressed

Similar to the `Written`, this event will also be called while file is writing to the destination stream but it will contain only the progress and percentage on how much the file is being uploaded.

### Started

This event will be called each time one of the selected file(s) has started the upload process.

### Ended

This event is fired after the file has ended the upload process. If there was no error it will have `Success` property set to true.

## Full example

In this example you can see the usage of all events, including the `Written` and `Progressed`. For your own use case you can just focus on `Changed` event.

```cs
<FileEdit Changed="@OnChanged" Written="@OnWritten" Progressed="@OnProgressed" />

@code{
    string fileContent;

    async Task OnChanged( FileChangedEventArgs e )
    {
        try
        {
            foreach ( var file in e.Files )
            {
                // A stream is going to be the destination stream we're writing to.                
                using ( var stream = new MemoryStream() )
                {
                    // Here we're telling the FileEdit where to write the upload result
                    await file.WriteToStreamAsync( stream );

                    // Once we reach this line it means the file is fully uploaded.
                    // In this case we're going to offset to the beginning of file
                    // so we can read it.
                    stream.Seek( 0, SeekOrigin.Begin );

                    // Use the stream reader to read the content of uploaded file,
                    // in this case we can assume it is a textual file.
                    using ( var reader = new StreamReader( stream ) )
                    {
                        fileContent = await reader.ReadToEndAsync();
                    }
                }
            }
        }
        catch ( Exception exc )
        {
            Console.WriteLine( exc.Message );
        }
        finally
        {
            this.StateHasChanged();
        }
    }

    void OnWritten( FileWrittenEventArgs e )
    {
        Console.WriteLine( $"File: {e.File.Name} Position: {e.Position} Data: {Convert.ToBase64String( e.Data )}" );
    }

    void OnProgressed( FileProgressedEventArgs e )
    {
        Console.WriteLine( $"File: {e.File.Name} Progress: {e.Percentage}" );
    }
}
```

## Attributes

| Name                  | Type      | Default     | Description                                                                                  |
|-----------------------|-----------|-------------|----------------------------------------------------------------------------------------------|
| Multiple              | boolean   | false       | Specifies that multiple files can be selected.                                               |
| Filter                | string    | null        | Types of files that the input accepts.                                                       |
| MaxMessageSize        | int       | 20480       | Max message size (in bytes) when uploading the file.                                         |
| Changed               | event     |             | Occurs every time the file(s) has changed.                                                   |
| Written               | event     |             | Occurs every time the part of file has being uploaded.                                       |
| Progressed            | event     |             | Notifies the progress of file being uploaded.                                                |
| Started               | event     |             | Occurs when an individual file upload has started.                                           |
| Ended                 | event     |             | Occurs when an individual file upload has ended.                                             |