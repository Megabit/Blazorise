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

## Examples

### WriteToStreamAsync

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

### OpenReadStream

Using `OpenReadStream` on the file you can process the file as it is streamed from the browser to your code, this is mirrors the API found in [ASP.NET Core Input File component](https://docs.microsoft.com/en-us/aspnet/core/blazor/file-uploads?view=aspnetcore-5.0), for example

```cs
<FileEdit Changed="@OnChanged" Written="@OnWritten" Progressed="@OnProgressed" />

@code{
    string fileContent;

    async Task OnChanged( FileChangedEventArgs e )
    {
        try
        {
            var file = e.Files.FirstOrDefault();
            if (file == null)
            {
                return;
            }

            var buffer = new byte[OneMb];
            using ( var bufferedStream = new BufferedStream( file.OpenReadStream( long.MaxValue ), OneMb ) )
            {
                int readCount = 0;
                int readBytes;
                while ( ( readBytes = await bufferedStream.ReadAsync( buffer, 0, OneMb ) ) > 0 )
                {
                    Console.WriteLine( $"Read:{readCount++} {readBytes / (double)OneMb} MB" );
                    // Do work on the first 1MB of data
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

### Reset

By default after each file upload has finished, file input will automatically reset to it's initial state. If you want this behavior disabled and control it manually you need to first set `AutoReset` to `false`. After that you can call `Reset()` every time you want the file input to be reset.

```cs
<FileEdit @ref="@fileEdit" AutoReset="false" ... />

@code{
    FileEdit fileEdit;

    Task OnSomeButtonClick()
    {
        return fileEdit.Reset();
    }
}
```

## Attributes

| Name                  | Type                      | Default     | Description                                                                                             |
|-----------------------|---------------------------|-------------|---------------------------------------------------------------------------------------------------------|
| Multiple              | boolean                   | false       | Specifies that multiple files can be selected.                                                          |
| Filter                | string                    | null        | Types of files that the input accepts.                                                                  |
| MaxMessageSize        | int                       | 20480       | Max message size (in bytes) when uploading the file.                                                    |
| Changed               | event                     |             | Occurs every time the file(s) has changed.                                                              |
| Written               | event                     |             | Occurs every time the part of file has being uploaded.                                                  |
| Progressed            | event                     |             | Notifies the progress of file being uploaded.                                                           |
| Started               | event                     |             | Occurs when an individual file upload has started.                                                      |
| Ended                 | event                     |             | Occurs when an individual file upload has ended.                                                        |
| AutoReset             | boolean                   | true        | If true file input will be automatically reset after it has being uploaded.                             |
| BrowseButtonLocalizer | `TextLocalizerHandler`    | `null`      | Function used to handle browse button localization that will override a default `ITextLocalizer`.       |