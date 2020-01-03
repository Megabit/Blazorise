#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public class ChangingEventArgs : CancelEventArgs
    {
        public ChangingEventArgs( string oldValue, string newValue )
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ChangingEventArgs( string oldValue, string newValue, bool cancel )
        {
            OldValue = oldValue;
            NewValue = newValue;
            Cancel = cancel;
        }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }

    public class ValidatingAllEventArgs : CancelEventArgs
    {
        public ValidatingAllEventArgs( bool cancel )
            : base( cancel )
        {
        }
    }

    public class ValidatorEventArgs : EventArgs
    {
        public ValidatorEventArgs( object value )
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value to check for validation.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the validation custom error message.
        /// </summary>
        public string ErrorText { get; set; }
    }

    public class ValidationSucceededEventArgs : EventArgs
    {
    }

    public class ValidationFailedEventArgs : EventArgs
    {
        public ValidationFailedEventArgs( string errorText )
        {
            ErrorText = errorText;
        }

        /// <summary>
        /// Gets the custom validation error message.
        /// </summary>
        public string ErrorText { get; }
    }

    public class ValidatedEventArgs : EventArgs
    {
        public ValidatedEventArgs( ValidationStatus status, string errorText )
        {
            Status = status;
            ErrorText = errorText;
        }

        /// <summary>
        /// Gets the validation result.
        /// </summary>
        public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets the custom validation error message.
        /// </summary>
        public string ErrorText { get; }
    }

    public class ValidationStartedEventArgs : EventArgs
    {
        public static new readonly ValidationStartedEventArgs Empty = new ValidationStartedEventArgs();

        public ValidationStartedEventArgs()
        {
        }
    }

    public class ValidationStatusChangedEventArgs : EventArgs
    {
        public static new readonly ValidationStatusChangedEventArgs Empty = new ValidationStatusChangedEventArgs( ValidationStatus.None );

        public ValidationStatusChangedEventArgs( ValidationStatus status, string message = null )
        {
            Status = status;
            Message = message;
        }

        /// <summary>
        /// Gets the validation result.
        /// </summary>
        public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets the custom validation message.
        /// </summary>
        public string Message { get; }
    }

    /// <summary>
    /// Supplies information about a mouse event that is being raised.
    /// </summary>
    public class BLMouseEventArgs : EventArgs
    {
        public BLMouseEventArgs( MouseButton button, long clicks, Point screen, Point client, bool ctrl, bool shift, bool alt )
        {
            Button = button;
            Clicks = clicks;
            Screen = screen;
            Client = client;
            CtrlKey = ctrl;
            ShiftKey = shift;
            AltKey = alt;
        }

        /// <summary>
        /// Gets which mouse button was pressed.
        /// </summary>
        public MouseButton Button { get; }

        /// <summary>
        /// Gets the number of times the mouse button was pressed and released.
        /// </summary>
        public long Clicks { get; }

        /// <summary>
        /// Gets the coordinate of the mouse pointer in global (screen) coordinates.
        /// </summary>
        public Point Screen { get; }

        /// <summary>
        /// Gets the coordinate of the mouse pointer in local (DOM content) coordinates.
        /// </summary>
        public Point Client { get; }

        /// <summary>
        /// true if the control key was down when the event was fired. false otherwise.
        /// </summary>
        public bool CtrlKey { get; }

        /// <summary>
        /// true if the shift key was down when the event was fired. false otherwise.
        /// </summary>
        public bool ShiftKey { get; }

        /// <summary>
        /// true if the alt key was down when the event was fired. false otherwise.
        /// </summary>
        public bool AltKey { get; }
    }

    /// <summary>
    /// Supplies the information about the modal visiblity state.
    /// </summary>
    public class ModalStateEventArgs : EventArgs
    {
        public ModalStateEventArgs( bool opened )
        {
            Opened = opened;
        }

        /// <summary>
        /// Gets that flag that indicates if the modal is opened.
        /// </summary>
        public bool Opened { get; }
    }

    /// <summary>
    /// Supplies the information about the alert visiblity state.
    /// </summary>
    public class AlertStateEventArgs : EventArgs
    {
        public AlertStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the alert is visible.
        /// </summary>
        public bool Visible { get; }
    }
}
