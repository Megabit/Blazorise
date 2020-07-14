﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorise.DataGrid
{
    /// <summary>
    /// States of the row items.
    /// </summary>
    public enum DataGridEditState
    {
        /// <summary>
        /// Default state which means the row value will just be showed.
        /// </summary>
        None,

        /// <summary>
        /// Row values will be visible in the new item edit mode.
        /// </summary>
        New,

        /// <summary>
        /// Row values will be visible in the row editing mode.
        /// </summary>
        Edit,
    }

    /// <summary>
    /// Specifes the grid editing modes.
    /// </summary>
    public enum DataGridEditMode
    {
        /// <summary>
        /// Values will be edited in the edit form.
        /// </summary>
        Form,

        /// <summary>
        /// Values will be edited within the inline edit row.
        /// </summary>
        Inline,

        /// <summary>
        /// Values will be edited in the modal dialog.
        /// </summary>
        Popup,
    }

    /// <summary>
    /// Defines the column editor type.
    /// </summary>
    public enum DataGridColumnType
    {
        /// <summary>
        /// Column can accept string values.
        /// </summary>
        Text,

        /// <summary>
        /// Column can accept only numeric values.
        /// </summary>
        Numeric,

        /// <summary>
        /// Column can accept only boolean values.
        /// </summary>
        Check,

        /// <summary>
        /// Column can accept only datetime values.
        /// </summary>
        Date,

        /// <summary>
        /// Column can be used to select multiple values.
        /// </summary>
        Select,

        /// <summary>
        /// Column is used only to represent commands like edit, save, etc.
        /// </summary>
        Command,
    }

    public enum DataGridFilterMethod
    {
        Contains,
        StartsWith,
        EndsWith,
        Equals,
        NotEquals,
    }

    /// <summary>
    /// Defines the supported aggregate calculation types.
    /// </summary>
    public enum DataGridAggregateType
    {
        /// <summary>
        /// Aggregation will not be calculated.
        /// </summary>
        None,

        /// <summary>
        /// Summary of numeric value.
        /// </summary>
        Sum,

        /// <summary>
        /// Average value of numeric value.
        /// </summary>
        Average,

        /// <summary>
        /// Min value of numeric value.
        /// </summary>
        Min,

        /// <summary>
        /// Max value of numeric value.
        /// </summary>
        Max,

        /// <summary>
        /// Count all values that are not null.
        /// </summary>
        Count,

        /// <summary>
        /// Count all boolean values that are true.
        /// </summary>
        TrueCount,

        /// <summary>
        /// Count all boolean values that are false.
        /// </summary>
        FalseCount,
    }
}
