#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

internal static class ReportWebApiSchemaBuilder
{
    #region Methods

    internal static ReportDataSourceSchema Create( object data, int maximumSchemaItems )
    {
        ReportDataSourceSchema schema = new();

        if ( data is IEnumerable enumerable and not string and not IDictionary )
        {
            schema.IsCollection = true;
            int itemCount = 0;

            foreach ( object item in enumerable )
            {
                MergeFields( schema.Fields, item, maximumSchemaItems );
                itemCount++;

                if ( itemCount >= maximumSchemaItems )
                    break;
            }
        }
        else
        {
            MergeFields( schema.Fields, data, maximumSchemaItems );
        }

        return schema;
    }

    private static void MergeFields( List<ReportDataSourceSchemaField> fields, object value, int maximumSchemaItems )
    {
        if ( value is not IDictionary dictionary )
            return;

        foreach ( DictionaryEntry entry in dictionary )
        {
            string name = Convert.ToString( entry.Key, CultureInfo.InvariantCulture );

            if ( string.IsNullOrWhiteSpace( name ) )
                continue;

            ReportDataSourceSchemaField incoming = CreateField( name, entry.Value, maximumSchemaItems );
            ReportDataSourceSchemaField existing = fields.FirstOrDefault( field => string.Equals( field.Name, name, StringComparison.OrdinalIgnoreCase ) );

            if ( existing is null )
            {
                fields.Add( incoming );
                continue;
            }

            MergeField( existing, incoming );
        }
    }

    private static ReportDataSourceSchemaField CreateField( string name, object value, int maximumSchemaItems )
    {
        ReportDataSourceSchemaField field = new()
        {
            Name = name,
            DisplayName = name,
            DataType = value?.GetType() ?? typeof( object ),
        };

        if ( value is IDictionary )
        {
            field.DataType = typeof( object );
            MergeFields( field.Fields, value, maximumSchemaItems );
        }
        else if ( value is IEnumerable enumerable and not string )
        {
            field.DataType = typeof( object );
            field.IsCollection = true;
            int itemCount = 0;

            foreach ( object item in enumerable )
            {
                MergeFields( field.Fields, item, maximumSchemaItems );
                itemCount++;

                if ( itemCount >= maximumSchemaItems )
                    break;
            }
        }

        return field;
    }

    private static void MergeField( ReportDataSourceSchemaField existing, ReportDataSourceSchemaField incoming )
    {
        if ( existing.DataType == typeof( object ) && existing.Fields.Count == 0 && !existing.IsCollection )
            existing.DataType = incoming.DataType;
        else if ( incoming.DataType != typeof( object ) && existing.DataType != incoming.DataType )
            existing.DataType = typeof( object );

        if ( existing.IsCollection != incoming.IsCollection )
            existing.DataType = typeof( object );

        existing.IsCollection |= incoming.IsCollection;

        foreach ( ReportDataSourceSchemaField incomingChild in incoming.Fields )
        {
            ReportDataSourceSchemaField existingChild = existing.Fields.FirstOrDefault( field => string.Equals( field.Name, incomingChild.Name, StringComparison.OrdinalIgnoreCase ) );

            if ( existingChild is null )
                existing.Fields.Add( incomingChild );
            else
                MergeField( existingChild, incomingChild );
        }
    }

    #endregion
}