#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Snackbar.Utils;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar;

public partial class SnackbarStack : BaseComponent
{
    #region Members

    private class SnackbarInfo
    {
        public SnackbarInfo( MarkupString message,
            string title,
            SnackbarColor color,
            string key,
            RenderFragment messageTemplate,
            bool showCloseButton,
            string closeButtonText,
            object closeButtonIcon,
            bool showActionButton,
            string actionButtonText,
            object actionButtonIcon,
            double? intervalBeforeClose,
            double? closeAnimationInterval,
            bool multiline )
        {
            Message = message;
            Title = title;
            Color = color;
            Key = key ?? Guid.NewGuid().ToString();
            MessageTemplate = messageTemplate;
            ShowCloseButton = showCloseButton;
            CloseButtonText = closeButtonText;
            CloseButtonIcon = closeButtonIcon;
            ShowActionButton = showActionButton;
            ActionButtonText = actionButtonText;
            ActionButtonIcon = actionButtonIcon;
            IntervalBeforeClose = intervalBeforeClose;
            CloseAnimationInterval = closeAnimationInterval;
            Multiline = multiline;
        }

        public MarkupString Message { get; }

        public string Title { get; }

        public SnackbarColor Color { get; }

        public string Key { get; }

        public RenderFragment MessageTemplate { get; }

        public bool ShowCloseButton { get; }

        public string CloseButtonText { get; }

        public object CloseButtonIcon { get; }

        public bool ShowActionButton { get; }

        public string ActionButtonText { get; }

        public object ActionButtonIcon { get; }

        public double? CloseAnimationInterval { get; }

        public double? IntervalBeforeClose { get; }

        public bool Visible { get; } = true;

        public bool Multiline { get; } = true;

    }

    private SnackbarStackLocation location = SnackbarStackLocation.Center;

    private readonly List<SnackbarInfo> snackbarInfos = new();

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "snackbar-stack" );
        builder.Append( $"snackbar-stack-{Location.GetName()}" );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Pushes the message to the stack to be shown as a snackbar.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="color">Message color.</param>
    /// <param name="options">Additional message options.</param>
    /// <returns>Returns awaitable task.</returns>
    public Task PushAsync( string message, SnackbarColor color = SnackbarColor.Default, Action<SnackbarOptions> options = null )
    {
        return PushAsync( (MarkupString)message, null, color, options );
    }

    /// <summary>
    /// Pushes the message to the stack to be shown as a snackbar.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="color">Message color.</param>
    /// <param name="options">Additional message options.</param>
    /// <returns>Returns awaitable task.</returns>
    public Task PushAsync( MarkupString message, SnackbarColor color = SnackbarColor.Default, Action<SnackbarOptions> options = null )
    {
        return PushAsync( message, null, color, options );
    }

    /// <summary>
    /// Pushes the message to the stack to be shown as a snackbar.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="title">Message caption.</param>
    /// <param name="color">Message color.</param>
    /// <param name="options">Additional message options.</param>
    /// <returns>Returns awaitable task.</returns>
    public Task PushAsync( string message, string title = null, SnackbarColor color = SnackbarColor.Default, Action<SnackbarOptions> options = null )
    {
        return PushAsync( (MarkupString)message, title, color, options );
    }

    /// <summary>
    /// Pushes the message to the stack to be shown as a snackbar.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="title">Message caption.</param>
    /// <param name="color">Message color.</param>
    /// <param name="options">Additional message options.</param>
    /// <returns>Returns awaitable task.</returns>
    public Task PushAsync( MarkupString message, string title = null, SnackbarColor color = SnackbarColor.Default, Action<SnackbarOptions> options = null )
    {
        var snackbarOptions = CreateDefaultOptions();
        options?.Invoke( snackbarOptions );

        snackbarInfos.Add( new( message, title, color,
            snackbarOptions.Key,
            snackbarOptions.MessageTemplate,
            snackbarOptions.ShowCloseButton,
            snackbarOptions.CloseButtonText,
            snackbarOptions.CloseButtonIcon,
            snackbarOptions.ShowActionButton,
            snackbarOptions.ActionButtonText,
            snackbarOptions.ActionButtonIcon,
            snackbarOptions.IntervalBeforeClose,
            snackbarOptions.CloseAnimationInterval,
            snackbarOptions.Multiline ) );

        return InvokeAsync( StateHasChanged );
    }

    private async Task OnSnackbarClosed( string key, SnackbarCloseReason closeReason )
    {
        var info = snackbarInfos.FirstOrDefault( x => x.Key == key );

        if ( info != null )
            snackbarInfos.Remove( info );

        await InvokeAsync( StateHasChanged );

        await Closed.InvokeAsync( new( key, closeReason ) );
    }

    protected virtual SnackbarOptions CreateDefaultOptions()
    {
        return new()
        {
            Key = IdGenerator.Generate,
            ShowCloseButton = true,
            IntervalBeforeClose = DefaultInterval
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the snackbar stack location.
    /// </summary>
    [Parameter]
    public SnackbarStackLocation Location
    {
        get => location;
        set
        {
            location = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the default interval (in milliseconds) after which the snackbars will be automatically closed (used if IntervalBeforeClose is not set on PushAsync call).
    /// </summary>
    [Parameter] public double? DefaultInterval { get; set; } = Constants.DefaultIntervalBeforeClose;

    /// <summary>
    /// If clicked on snackbar, a close action will be delayed by increasing the <see cref="DefaultInterval"/> time.
    /// </summary>
    [Parameter] public bool DelayCloseOnClick { get; set; }

    /// <summary>
    /// Defines the interval(in milliseconds) by which the snackbar will be delayed from closing.
    /// </summary>
    [Parameter] public double? DelayCloseOnClickInterval { get; set; }

    /// <summary>
    /// Defines a text to show for snackbar close button. Leave as null to not show it!
    /// </summary>
    [Parameter] public string CloseButtonText { get; set; }

    /// <summary>
    /// Defines an icon to show for snackbar close button. Leave as null to not show it!
    /// </summary>
    [Parameter] public object CloseButtonIcon { get; set; }

    /// <summary>
    /// Defines a text to show for snackbar action button. Leave as null to not show it!
    /// </summary>
    [Parameter] public string ActionButtonText { get; set; }

    /// <summary>
    /// Defines an icon to show for snackbar action button. Leave as null to not show it!
    /// </summary>
    [Parameter] public object ActionButtonIcon { get; set; }

    /// <summary>
    /// Occurs after the snackbar has closed.
    /// </summary>
    [Parameter] public EventCallback<SnackbarClosedEventArgs> Closed { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SnackbarStack"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}