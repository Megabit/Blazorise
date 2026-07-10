#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides the current declarative PDF document context.
/// </summary>
public sealed class PdfDocumentContext
{
    /// <summary>
    /// Initializes a new PDF document context.
    /// </summary>
    /// <param name="definition">The current document definition.</param>
    public PdfDocumentContext( PdfDocumentDefinition definition )
    {
        Definition = definition;
    }

    /// <summary>
    /// Registers a document-scoped font family.
    /// </summary>
    /// <param name="font">Font family.</param>
    public void RegisterFont( FontFamily font )
    {
        if ( string.IsNullOrWhiteSpace( font?.Name ) )
            return;

        Definition.Fonts ??= [];

        int registeredIndex = Definition.Fonts.FindIndex( x => ReferenceEquals( x, font ) );
        int existingIndex = Definition.Fonts.FindIndex( x =>
            !ReferenceEquals( x, font )
            && string.Equals( x.Name, font.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( registeredIndex >= 0 )
        {
            if ( existingIndex >= 0 && existingIndex != registeredIndex )
            {
                Definition.Fonts[existingIndex] = font;
                Definition.Fonts.RemoveAt( registeredIndex );
            }
        }
        else if ( existingIndex >= 0 )
            Definition.Fonts[existingIndex] = font;
        else
            Definition.Fonts.Add( font );
    }

    /// <summary>
    /// Unregisters a document-scoped font family.
    /// </summary>
    /// <param name="font">Font family.</param>
    public void UnregisterFont( FontFamily font )
    {
        if ( font is not null )
            Definition.Fonts?.Remove( font );
    }

    /// <summary>
    /// Current document definition.
    /// </summary>
    public PdfDocumentDefinition Definition { get; }
}