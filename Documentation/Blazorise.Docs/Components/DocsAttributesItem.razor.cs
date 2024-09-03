#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsAttributesItem : IDisposable
{
    #region Members

    private bool disposedValue;

    #endregion

    #region Methods

    protected override Task OnInitializedAsync()
    {
        if ( ParentDocsAttributes is not null )
        {
            ParentDocsAttributes.AddItem( this );
        }
        return base.OnInitializedAsync();
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( !disposedValue )
        {
            if ( disposing )
            {
                if ( ParentDocsAttributes is not null )
                    ParentDocsAttributes.RemoveItem( this );
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
    }

    #endregion

    #region Properties

    internal string DefaultClassNames => Default switch
    {
        "true" => "b-attribute-token boolean",
        "false" => "b-attribute-token boolean",
        "null" => "b-attribute-token keyword",
        "None" => "b-attribute-token keyword",
        _ => "b-attribute-token string",
    };

    [CascadingParameter] public DocsAttributes ParentDocsAttributes { get; set; }

    [Parameter] public string Name { get; set; }

    [Parameter] public string Type { get; set; }

    [Parameter] public bool TypeTag { get; set; }

    [Parameter] public string Default { get; set; }

    [Parameter] public string ObsoleteMessage { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}