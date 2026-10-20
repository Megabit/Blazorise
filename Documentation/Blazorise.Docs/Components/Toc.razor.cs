#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class Toc : IDisposable
{
    #region Methods

    protected override void OnInitialized()
    {
        ParentToc.Changed += OnHeadingsChanged;

        base.OnInitialized();
    }

    private void OnHeadingsChanged() => _ = InvokeAsync( StateHasChanged );

    public void Dispose() => ParentToc.Changed -= OnHeadingsChanged;

    #endregion

    #region Properties

    [CascadingParameter] private DocsTocNode ParentToc { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    #endregion
}