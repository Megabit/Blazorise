#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsMethodsItem : IDisposable
{
    #region Members

    private bool disposedValue;

    #endregion

    #region Methods

    protected override Task OnInitializedAsync()
    {
        if ( ParentDocsMethods is not null )
        {
            ParentDocsMethods.AddItem( this );
        }
        return base.OnInitializedAsync();
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( !disposedValue )
        {
            if ( disposing )
            {
                if ( ParentDocsMethods is not null )
                    ParentDocsMethods.RemoveItem( this );
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

    internal string DefaultClassNames => Parameters switch
    {
        "true" => "b-method-token boolean",
        "false" => "b-method-token boolean",
        "null" => "b-method-token keyword",
        "None" => "b-method-token keyword",
        _ => "b-method-token string",
    };

    [CascadingParameter] public DocsMethods ParentDocsMethods { get; set; }

    [Parameter] public string Name { get; set; }

    [Parameter] public string ReturnType { get; set; }

    [Parameter] public bool ReturnTypeTag { get; set; }

    [Parameter] public string Parameters { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}