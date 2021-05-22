﻿namespace Blazorise
{
    /// <summary>
    /// Hints at the type of data that might be entered by the user while editing the <see cref="DateEdit{TValue}"/> component.
    /// </summary>
    public enum DateInputMode
    {
        /// <summary>
        /// Only date is allowed to be entered.
        /// </summary>
        Date,

        /// <summary>
        /// Both date and time are allowed to be entered.
        /// </summary>
        DateTime,

        /// <summary>
        /// Allowed to select only year and month.
        /// 
        /// Note that not all browser supports this mode, see <see href="https://caniuse.com/input-datetime" /> for more info.
        /// </summary>
        Month,
    }
}
