#region Using directives
using System;
using System.Globalization;
using Blazorise.Gantt.Utilities;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Maps item fields used by the Gantt chart.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttPropertyMapper<TItem>
{
    #region Members

    private Func<TItem, object> getIdFunc;
    private Action<TItem, object> setIdFunc;

    private Func<TItem, object> getParentIdFunc;
    private Action<TItem, object> setParentIdFunc;

    private Func<TItem, string> getTitleFunc;
    private Action<TItem, string> setTitleFunc;

    private Func<TItem, string> getDescriptionFunc;
    private Action<TItem, string> setDescriptionFunc;

    private Func<TItem, DateTime> getStartFunc;
    private Action<TItem, DateTime> setStartFunc;

    private Func<TItem, DateTime> getEndFunc;
    private Action<TItem, DateTime> setEndFunc;

    private Func<TItem, object> getDurationFunc;
    private Action<TItem, object> setDurationFunc;
    private Type durationType;

    private Func<TItem, object> getProgressFunc;
    private Action<TItem, object> setProgressFunc;
    private Type progressType;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new mapper instance.
    /// </summary>
    public GanttPropertyMapper( Gantt<TItem> gantt )
    {
        if ( !string.IsNullOrEmpty( gantt.IdField ) && typeof( TItem ).GetProperty( gantt.IdField )?.PropertyType is not null )
        {
            getIdFunc = GanttFunctionCompiler.CreateValueGetter<TItem>( gantt.IdField );
            setIdFunc = GanttFunctionCompiler.CreateValueSetter<TItem>( gantt.IdField );
        }

        if ( !string.IsNullOrEmpty( gantt.ParentIdField ) && typeof( TItem ).GetProperty( gantt.ParentIdField )?.PropertyType is not null )
        {
            getParentIdFunc = GanttFunctionCompiler.CreateValueGetter<TItem>( gantt.ParentIdField );
            setParentIdFunc = GanttFunctionCompiler.CreateValueSetter<TItem>( gantt.ParentIdField );
        }

        if ( !string.IsNullOrEmpty( gantt.TitleField ) && typeof( TItem ).GetProperty( gantt.TitleField )?.PropertyType is not null )
        {
            getTitleFunc = GanttFunctionCompiler.CreateValueGetter<TItem, string>( gantt.TitleField );
            setTitleFunc = GanttFunctionCompiler.CreateValueSetter<TItem, string>( gantt.TitleField );
        }

        if ( !string.IsNullOrEmpty( gantt.DescriptionField ) && typeof( TItem ).GetProperty( gantt.DescriptionField )?.PropertyType is not null )
        {
            getDescriptionFunc = GanttFunctionCompiler.CreateValueGetter<TItem, string>( gantt.DescriptionField );
            setDescriptionFunc = GanttFunctionCompiler.CreateValueSetter<TItem, string>( gantt.DescriptionField );
        }

        if ( !string.IsNullOrEmpty( gantt.StartField ) && typeof( TItem ).GetProperty( gantt.StartField )?.PropertyType is not null )
        {
            getStartFunc = GanttFunctionCompiler.CreateValueGetter<TItem, DateTime>( gantt.StartField );
            setStartFunc = GanttFunctionCompiler.CreateValueSetter<TItem, DateTime>( gantt.StartField );
        }

        if ( !string.IsNullOrEmpty( gantt.EndField ) && typeof( TItem ).GetProperty( gantt.EndField )?.PropertyType is not null )
        {
            getEndFunc = GanttFunctionCompiler.CreateValueGetter<TItem, DateTime>( gantt.EndField );
            setEndFunc = GanttFunctionCompiler.CreateValueSetter<TItem, DateTime>( gantt.EndField );
        }

        var durationProperty = !string.IsNullOrEmpty( gantt.DurationField )
            ? typeof( TItem ).GetProperty( gantt.DurationField )
            : null;

        if ( durationProperty?.PropertyType is not null )
        {
            var underlyingDurationType = Nullable.GetUnderlyingType( durationProperty.PropertyType ) ?? durationProperty.PropertyType;

            if ( IsSupportedDurationType( underlyingDurationType ) )
            {
                durationType = durationProperty.PropertyType;
                getDurationFunc = GanttFunctionCompiler.CreateValueGetter<TItem>( gantt.DurationField );
                setDurationFunc = GanttFunctionCompiler.CreateValueSetter<TItem>( gantt.DurationField );
            }
        }

        var progressProperty = !string.IsNullOrEmpty( gantt.ProgressField )
            ? typeof( TItem ).GetProperty( gantt.ProgressField )
            : null;

        if ( progressProperty?.PropertyType is not null )
        {
            var underlyingProgressType = Nullable.GetUnderlyingType( progressProperty.PropertyType ) ?? progressProperty.PropertyType;

            if ( IsSupportedProgressType( underlyingProgressType ) )
            {
                progressType = progressProperty.PropertyType;
                getProgressFunc = GanttFunctionCompiler.CreateValueGetter<TItem>( gantt.ProgressField );
                setProgressFunc = GanttFunctionCompiler.CreateValueSetter<TItem>( gantt.ProgressField );
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets item id.
    /// </summary>
    public object GetId( TItem item ) => getIdFunc?.Invoke( item );

    /// <summary>
    /// Sets item id.
    /// </summary>
    public void SetId( TItem item, object value ) => setIdFunc?.Invoke( item, value );

    /// <summary>
    /// Gets parent id.
    /// </summary>
    public object GetParentId( TItem item ) => getParentIdFunc?.Invoke( item );

    /// <summary>
    /// Sets parent id.
    /// </summary>
    public void SetParentId( TItem item, object value ) => setParentIdFunc?.Invoke( item, value );

    /// <summary>
    /// Gets title.
    /// </summary>
    public string GetTitle( TItem item ) => getTitleFunc?.Invoke( item );

    /// <summary>
    /// Sets title.
    /// </summary>
    public void SetTitle( TItem item, string value ) => setTitleFunc?.Invoke( item, value );

    /// <summary>
    /// Gets description.
    /// </summary>
    public string GetDescription( TItem item ) => getDescriptionFunc?.Invoke( item );

    /// <summary>
    /// Sets description.
    /// </summary>
    public void SetDescription( TItem item, string value ) => setDescriptionFunc?.Invoke( item, value );

    /// <summary>
    /// Gets start date.
    /// </summary>
    public DateTime GetStart( TItem item ) => getStartFunc?.Invoke( item ) ?? DateTime.MinValue;

    /// <summary>
    /// Sets start date.
    /// </summary>
    public void SetStart( TItem item, DateTime value ) => setStartFunc?.Invoke( item, value );

    /// <summary>
    /// Gets end date.
    /// </summary>
    public DateTime GetEnd( TItem item ) => getEndFunc?.Invoke( item ) ?? DateTime.MinValue;

    /// <summary>
    /// Sets end date.
    /// </summary>
    public void SetEnd( TItem item, DateTime value ) => setEndFunc?.Invoke( item, value );

    /// <summary>
    /// Gets duration in days.
    /// </summary>
    public int GetDuration( TItem item )
        => ToDurationInDays( getDurationFunc?.Invoke( item ) );

    /// <summary>
    /// Sets duration in days.
    /// </summary>
    public void SetDuration( TItem item, int value )
    {
        if ( setDurationFunc is null )
            return;

        var normalizedValue = Math.Max( 1, value );
        var convertedValue = ConvertDurationToTargetType( normalizedValue, durationType );
        setDurationFunc.Invoke( item, convertedValue );
    }

    /// <summary>
    /// Gets progress value.
    /// </summary>
    public object GetProgress( TItem item ) => getProgressFunc?.Invoke( item );

    /// <summary>
    /// Sets progress value.
    /// </summary>
    public void SetProgress( TItem item, object value ) => setProgressFunc?.Invoke( item, value );

    /// <summary>
    /// Gets normalized progress percentage in range 0..100.
    /// </summary>
    public double? GetProgressPercentage( TItem item )
    {
        if ( !HasProgress || item is null )
            return null;

        if ( !ValueUtils.TryConvertToDouble( getProgressFunc?.Invoke( item ), out var progressValue ) )
            return null;

        if ( IsFractionScale( progressValue ) )
            progressValue *= 100d;

        return Math.Max( 0d, Math.Min( 100d, progressValue ) );
    }

    /// <summary>
    /// Determines whether current item's progress uses fraction scale (0..1).
    /// </summary>
    public bool IsProgressFractionScale( TItem item )
    {
        if ( !HasProgress || item is null || !IsFractionCompatibleProgressType( progressType ) )
            return false;

        if ( !ValueUtils.TryConvertToDouble( getProgressFunc?.Invoke( item ), out var progressValue ) )
            return false;

        return IsFractionScale( progressValue );
    }

    /// <summary>
    /// Sets progress by normalized percentage value.
    /// </summary>
    public void SetProgressPercentage( TItem item, double percentage, bool useFractionScale )
    {
        if ( !HasProgress || setProgressFunc is null || item is null )
            return;

        var normalizedPercentage = Math.Max( 0d, Math.Min( 100d, percentage ) );
        var effectiveFractionScale = useFractionScale && IsFractionCompatibleProgressType( progressType );
        var rawValue = effectiveFractionScale
            ? normalizedPercentage / 100d
            : normalizedPercentage;
        var convertedValue = ConvertProgressToTargetType( rawValue, progressType );

        setProgressFunc.Invoke( item, convertedValue );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether Id is mapped.
    /// </summary>
    public bool HasId => getIdFunc is not null;

    /// <summary>
    /// Indicates whether ParentId is mapped.
    /// </summary>
    public bool HasParentId => getParentIdFunc is not null;

    /// <summary>
    /// Indicates whether Title is mapped.
    /// </summary>
    public bool HasTitle => getTitleFunc is not null;

    /// <summary>
    /// Indicates whether Description is mapped.
    /// </summary>
    public bool HasDescription => getDescriptionFunc is not null;

    /// <summary>
    /// Indicates whether Start is mapped.
    /// </summary>
    public bool HasStart => getStartFunc is not null;

    /// <summary>
    /// Indicates whether End is mapped.
    /// </summary>
    public bool HasEnd => getEndFunc is not null;

    /// <summary>
    /// Indicates whether Duration is mapped.
    /// </summary>
    public bool HasDuration => getDurationFunc is not null && setDurationFunc is not null;

    /// <summary>
    /// Indicates whether Progress is mapped.
    /// </summary>
    public bool HasProgress => getProgressFunc is not null;

    #endregion

    #region Helpers

    private static int ToDurationInDays( object value )
    {
        if ( value is null )
            return 0;

        switch ( value )
        {
            case byte byteValue:
                return byteValue;
            case short shortValue:
                return shortValue;
            case int intValue:
                return intValue;
            case long longValue:
                return (int)Math.Max( int.MinValue, Math.Min( int.MaxValue, longValue ) );
            case float floatValue:
                return (int)Math.Max( int.MinValue, Math.Min( int.MaxValue, Math.Ceiling( floatValue ) ) );
            case double doubleValue:
                return (int)Math.Max( int.MinValue, Math.Min( int.MaxValue, Math.Ceiling( doubleValue ) ) );
            case decimal decimalValue:
                return (int)Math.Max( int.MinValue, Math.Min( int.MaxValue, Math.Ceiling( decimalValue ) ) );
            case string stringValue when int.TryParse( stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInt ):
                return parsedInt;
            case string stringValue when double.TryParse( stringValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsedDouble ):
                return (int)Math.Max( int.MinValue, Math.Min( int.MaxValue, Math.Ceiling( parsedDouble ) ) );
            case IConvertible convertible:
                try
                {
                    var convertedValue = convertible.ToDouble( CultureInfo.InvariantCulture );
                    return (int)Math.Max( int.MinValue, Math.Min( int.MaxValue, Math.Ceiling( convertedValue ) ) );
                }
                catch
                {
                    return 0;
                }
            default:
                return 0;
        }
    }

    private static object ConvertDurationToTargetType( int value, Type targetType )
    {
        if ( targetType is null )
            return value;

        var nonNullableType = Nullable.GetUnderlyingType( targetType ) ?? targetType;

        if ( nonNullableType == typeof( int ) )
            return value;

        if ( nonNullableType == typeof( long ) )
            return (long)value;

        if ( nonNullableType == typeof( short ) )
            return (short)Math.Min( short.MaxValue, value );

        if ( nonNullableType == typeof( byte ) )
            return (byte)Math.Min( byte.MaxValue, value );

        if ( nonNullableType == typeof( float ) )
            return (float)value;

        if ( nonNullableType == typeof( double ) )
            return (double)value;

        if ( nonNullableType == typeof( decimal ) )
            return (decimal)value;

        if ( nonNullableType == typeof( string ) )
            return value.ToString( CultureInfo.InvariantCulture );

        return Convert.ChangeType( value, nonNullableType, CultureInfo.InvariantCulture );
    }

    private static bool IsSupportedDurationType( Type type )
    {
        return type == typeof( byte )
            || type == typeof( short )
            || type == typeof( int )
            || type == typeof( long )
            || type == typeof( float )
            || type == typeof( double )
            || type == typeof( decimal )
            || type == typeof( string );
    }

    private static object ConvertProgressToTargetType( double value, Type targetType )
    {
        if ( targetType is null )
            return value;

        var nonNullableType = Nullable.GetUnderlyingType( targetType ) ?? targetType;

        if ( nonNullableType == typeof( double ) )
            return value;

        if ( nonNullableType == typeof( float ) )
            return (float)value;

        if ( nonNullableType == typeof( decimal ) )
            return (decimal)value;

        var roundedValue = Math.Round( value, MidpointRounding.AwayFromZero );

        if ( nonNullableType == typeof( int ) )
            return (int)roundedValue;

        if ( nonNullableType == typeof( long ) )
            return (long)roundedValue;

        if ( nonNullableType == typeof( short ) )
            return (short)Math.Max( short.MinValue, Math.Min( short.MaxValue, roundedValue ) );

        if ( nonNullableType == typeof( byte ) )
            return (byte)Math.Max( byte.MinValue, Math.Min( byte.MaxValue, roundedValue ) );

        if ( nonNullableType == typeof( string ) )
            return value.ToString( "G29", CultureInfo.InvariantCulture );

        return Convert.ChangeType( value, nonNullableType, CultureInfo.InvariantCulture );
    }

    private static bool IsSupportedProgressType( Type type )
    {
        return type == typeof( byte )
            || type == typeof( short )
            || type == typeof( int )
            || type == typeof( long )
            || type == typeof( float )
            || type == typeof( double )
            || type == typeof( decimal )
            || type == typeof( string );
    }

    private static bool IsFractionCompatibleProgressType( Type type )
    {
        if ( type is null )
            return false;

        var nonNullableType = Nullable.GetUnderlyingType( type ) ?? type;

        return nonNullableType == typeof( float )
            || nonNullableType == typeof( double )
            || nonNullableType == typeof( decimal )
            || nonNullableType == typeof( string );
    }

    private static bool IsFractionScale( double progressValue )
        => progressValue > 0d && progressValue <= 1d;

    #endregion
}