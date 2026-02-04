#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Larger text that can be placed in the <see cref="ModalHeader"/>.
/// </summary>
public partial class ModalTitle : BaseComponent, IDisposable
{
    #region Members

    private HeadingSize size = HeadingSize.Is4;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentModal?.NotifyModalTitleInitialized( this );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentModal?.NotifyModalTitleRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ModalTitle() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the title tag name.
    /// </summary>
    protected string TagName => $"h{SizeNumber}";

    /// <summary>
    /// Gets the heading size number.
    /// </summary>
    protected string SizeNumber => Size switch
    {
        HeadingSize.Is1 => "1",
        HeadingSize.Is2 => "2",
        HeadingSize.Is3 => "3",
        HeadingSize.Is4 => "4",
        HeadingSize.Is5 => "5",
        HeadingSize.Is6 => "6",
        _ => "3",
    };

    /// <summary>
    /// Gets or sets the cascaded parent modal component.
    /// </summary>
    [CascadingParameter] protected Modal ParentModal { get; set; }

    /// <summary>
    /// Gets or sets the title size.
    /// </summary>
    [Parameter]
    public HeadingSize Size
    {
        get => size;
        set
        {
            size = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ModalTitle"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}