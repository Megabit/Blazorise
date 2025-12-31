#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main wrapper for the content area of the modal component.
/// </summary>
public partial class ModalContent : BaseComponent, IDisposable
{
    #region Members

    private bool centered;

    private bool scrollable;

    private ModalSize modalSize = ModalSize.Default;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentModal is not null )
        {
            ParentModal._Opened += OnModalOpened;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( ParentModal is not null )
            {
                ParentModal._Opened -= OnModalOpened;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ModalContent( AsDialog ) );
        builder.Append( ClassProvider.ModalContentSize( Size ) );
        builder.Append( ClassProvider.ModalContentCentered( Centered ) );
        builder.Append( ClassProvider.ModalContentScrollable( Scrollable ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        var parentCentered = ParentModal?.Centered ?? false;
        var parentScrollable = ParentModal?.Scrollable ?? false;
        var parentSize = ParentModal?.Size ?? ModalSize.Default;

        if ( centered != parentCentered || scrollable != parentScrollable || modalSize != parentSize )
        {
            centered = parentCentered;
            scrollable = parentScrollable;
            modalSize = parentSize;

            DirtyClasses();
        }

        base.OnParametersSet();
    }

    internal void NotifyHasModalHeader()
    {
        HasModalHeader = true;
    }

    internal void NotifyHasModalBody()
    {
        HasModalBody = true;
    }

    internal void NotifyHasModalFooter()
    {
        HasModalFooter = true;
    }

    private void OnModalOpened()
    {
        ExecuteAfterRender( FocusTrapRef.SetFocus );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if modal contains the <see cref="ModalHeader"/> component.
    /// </summary>
    protected bool HasModalHeader { get; set; }

    /// <summary>
    /// True if modal contains the <see cref="ModalBody"/> component.
    /// </summary>
    protected bool HasModalBody { get; set; }

    /// <summary>
    /// True if modal contains the <see cref="ModalFooter"/> component.
    /// </summary>
    protected bool HasModalFooter { get; set; }

    /// <summary>
    /// Makes the modal as classic dialog with header, body and footer.
    /// 
    /// Currently used only by bulma https://bulma.io/documentation/components/modal/
    /// </summary>
    protected virtual bool AsDialog => false;

    /// <summary>
    /// True if modal should keep the input focus.
    /// </summary>
    protected bool IsFocusTrap => ParentModal?.FocusTrap ?? Options?.ModalFocusTrap ?? true;

    /// <summary>
    /// Gets or sets the <see cref="FocusTrap"/> reference.
    /// </summary>
    protected FocusTrap FocusTrapRef { get; set; }

    /// <summary>
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    /// <summary>
    /// Gets whether the parent modal is centered.
    /// </summary>
    public bool Centered => centered;

    /// <summary>
    /// Gets whether the parent modal is scrollable.
    /// </summary>
    public bool Scrollable => scrollable;

    /// <summary>
    /// Gets the size defined on the parent modal.
    /// </summary>
    public virtual ModalSize Size => modalSize;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ModalContent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="Modal"/> component.
    /// </summary>
    [CascadingParameter] protected Modal ParentModal { get; set; }

    #endregion
}