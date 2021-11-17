﻿namespace Blazorise.States
{
    /// <summary>
    /// Holds the information about the current state of the <see cref="Modal"/> component.
    /// </summary>
    public record ModalState
    {
        /// <summary>
        /// Defines the visibility of modal dialog.
        /// </summary>
        public bool Visible { get; init; }
    }
}
