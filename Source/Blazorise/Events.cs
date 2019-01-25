#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class ConvertEditValueEventArgs : EventArgs
    {
        public ConvertEditValueEventArgs( object value, bool handled )
        {
            Value = value;
            Handled = handled;
        }

        public object Value { get; set; }

        public bool Handled { get; set; }
    }

    public class FormatInfo
    {
        public FormatType FormatType { get; set; }

        public IFormatProvider Format { get; set; }

        public string FormatString { get; set; }
    }

    public enum FormatType
    {
        None = 0,
        Numeric = 1,
        DateTime = 2,
        Custom = 3,
    }

    public enum MaskType
    {
        None,
        Numeric = 1,
        DateTime = 2,
        Regex = 3,
    }

    public class CustomFormat : IFormatProvider, ICustomFormatter
    {
        public object GetFormat( Type formatType )
        {
            if ( formatType == typeof( ICustomFormatter ) )
                return this;
            else
                return null;
        }

        public string Format( string format, object arg, IFormatProvider formatProvider )
        {
            if ( format == null )
            {
                if ( arg is IFormattable )
                    return ( (IFormattable)arg ).ToString( format, formatProvider );
                else
                    return arg.ToString();
            }
            if ( format == "B" )
                return Convert.ToString( Convert.ToInt32( arg ), 2 );
            else
                return arg.ToString();
        }
    }
}
