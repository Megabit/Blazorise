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
    /// <summary>
    /// Provides data for the <see cref="Modal.Closing"/> event.
    /// </summary>
    public class ModalClosingEventArgs : CancelEventArgs
    {
        public ModalClosingEventArgs( bool cancel, CloseReason closeReason )
            : base( cancel )
        {
            CloseReason = closeReason;
        }

        /// <summary>
        /// Gets a value that indicates why the modal is being closed.
        /// </summary>
        public CloseReason CloseReason { get; }
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
        public ModalStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the modal is opened.
        /// </summary>
        public bool Visible { get; }
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

    /// <summary>
    /// Supplies the information about the dropdown state.
    /// </summary>
    public class DropdownStateEventArgs : EventArgs
    {
        public DropdownStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the dropdown is opened.
        /// </summary>
        public bool Visible { get; }
    }

    /// <summary>
    /// Supplies the information about the bar-dropdown state.
    /// </summary>
    public class BarDropdownStateEventArgs : EventArgs
    {
        public BarDropdownStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the dropdown is opened.
        /// </summary>
        public bool Visible { get; }
    }

    /// <summary>
    /// Supplies the information about the bar state.
    /// </summary>
    public class BarStateEventArgs : EventArgs
    {
        public BarStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the bar is opened.
        /// </summary>
        public bool Visible { get; }
    }

    /// <summary>
    /// Supplies the information about the selected tab.
    /// </summary>
    public class TabsStateEventArgs : EventArgs
    {
        public TabsStateEventArgs( string tabName )
        {
            TabName = tabName;
        }

        /// <summary>
        /// Gets the selected tab name.
        /// </summary>
        public string TabName { get; }
    }

    /// <summary>
    /// Supplies the information about the selected panel.
    /// </summary>
    public class TabsContentStateEventArgs : EventArgs
    {
        public TabsContentStateEventArgs( string panelName )
        {
            PanelName = panelName;
        }

        /// <summary>
        /// Gets the selected panel name.
        /// </summary>
        public string PanelName { get; }
    }

    /// <summary>
    /// Supplies the information about the selected files ready to be uploaded.
    /// </summary>
    public class FileChangedEventArgs : EventArgs
    {
        public FileChangedEventArgs( IFileEntry[] files )
        {
            Files = files;
        }

        /// <summary>
        /// Gets the list of selected files.
        /// </summary>
        public IFileEntry[] Files { get; }
    }

    /// <summary>
    /// Supplies the information about the data being written while uploading.
    /// </summary>
    public class FileWrittenEventArgs : EventArgs
    {
        public FileWrittenEventArgs( IFileEntry file, long position, byte[] data )
        {
            File = file;
            Position = position;
            Data = data;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }

        /// <summary>
        /// Gets the current position offset based on the original data source.
        /// </summary>
        public long Position { get; }

        /// <summary>
        /// Gets the data buffer.
        /// </summary>
        public byte[] Data { get; }
    }

    /// <summary>
    /// Provides the progress state of uploaded file.
    /// </summary>
    public class FileProgressedEventArgs : EventArgs
    {
        public FileProgressedEventArgs( IFileEntry file, double progress )
        {
            File = file;
            Progress = progress;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }

        /// <summary>
        /// Gets the total progress in the range from 0 to 1.
        /// </summary>
        public double Progress { get; }

        /// <summary>
        /// Gets the total progress in the range from 0 to 100.
        /// </summary>
        public double Percentage => Progress * 100d;
    }

    /// <summary>
    /// Provides the information about the file started to be uploaded.
    /// </summary>
    public class FileStartedEventArgs : EventArgs
    {
        public FileStartedEventArgs( IFileEntry file )
        {
            File = file;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }
    }

    /// <summary>
    /// Provides the information about the file ended uploading.
    /// </summary>
    public class FileEndedEventArgs : EventArgs
    {
        public FileEndedEventArgs( IFileEntry file, bool success )
        {
            File = file;
            Success = success;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public IFileEntry File { get; }

        /// <summary>
        /// Gets the value indicating if file has finished successfully.
        /// </summary>
        public bool Success { get; }
    }

    /// <summary>
    /// Provides the information about the currently checked radio.
    /// </summary>
    public class RadioCheckedChangedEventArgs : EventArgs
    {
        public RadioCheckedChangedEventArgs( object value )
        {
            Value = value;
        }

        /// <summary>
        /// Gets the checked radio value.
        /// </summary>
        public object Value { get; }
    }
}
