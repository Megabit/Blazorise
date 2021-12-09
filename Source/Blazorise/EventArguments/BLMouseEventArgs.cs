#region Using directives
using System;
using System.Drawing;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies information about a mouse event that is being raised.
    /// </summary>
    public class BLMouseEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="BLMouseEventArgs"/> constructor.
        /// </summary>
        /// <param name="button">Mouse button that was pressed.</param>
        /// <param name="clicks">Number of times the mouse button was pressed and released.</param>
        /// <param name="screen">Coordinate of the mouse pointer in global (screen) coordinates.</param>
        /// <param name="client">Coordinate of the mouse pointer in local (DOM content) coordinates.</param>
        /// <param name="ctrl">true if the control key was down when the event was fired. false otherwise.</param>
        /// <param name="shift">true if the shift key was down when the event was fired. false otherwise.</param>
        /// <param name="alt">true if the alt key was down when the event was fired. false otherwise.</param>
        /// <param name="meta">true if the meta key was down when the event was fired. false otherwise.</param>
        public BLMouseEventArgs( MouseButton button, long clicks, Point screen, Point client, bool ctrl, bool shift, bool alt, bool meta )
        {
            Button = button;
            Clicks = clicks;
            Screen = screen;
            Client = client;
            CtrlKey = ctrl;
            ShiftKey = shift;
            AltKey = alt;
            MetaKey = meta;
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

        /// <summary>
        /// true if the meta key was down when the event was fired. false otherwise.
        /// </summary>
        public bool MetaKey { get; }
    }
}
