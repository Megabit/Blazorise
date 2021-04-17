﻿namespace Blazorise
{
    /// <summary>
    /// Custom input roles.
    /// </summary>
    public enum ControlRole
    {
        /// <summary>
        /// Control doesn't have any special rule.
        /// </summary>
        None,

        /// <summary>
        /// Control is meant to be used with <see cref="Check{TValue}"/> component.
        /// </summary>
        Check,

        /// <summary>
        /// Control is meant to be used with <see cref="Radio{TValue}"/> component.
        /// </summary>
        Radio,

        /// <summary>
        /// Control is meant to be used with <see cref="Switch{TValue}"/> component.
        /// </summary>
        Switch,

        /// <summary>
        /// Control is meant to be used with <see cref="FileEdit"/> component.
        /// </summary>
        File,

        /// <summary>
        /// Control is meant to be used with <see cref="TextEdit"/> component.
        /// </summary>
        Text,
    }
}
