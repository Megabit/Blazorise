#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDataCommandService
{
    #region Methods

    internal async Task ConnectDataSourceAsync( ReportDefinition definition, object data, ReportDataSourceDefinition dataSource, Func<ReportDefinition, bool, Task> resolveDataSources )
    {
        if ( definition is null || dataSource is null || string.IsNullOrWhiteSpace( dataSource.Name ) )
            return;

        if ( definition.DataSources is null )
            definition.DataSources = [];

        if ( IsInMemoryProvider( dataSource.ProviderType )
            && dataSource.Data is null )
        {
            dataSource.Data = data;
        }

        int existingIndex = definition.DataSources.FindIndex( source =>
            string.Equals( source.Id, dataSource.Id, StringComparison.Ordinal )
            || string.Equals( source.Name, dataSource.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingIndex >= 0 )
            definition.DataSources[existingIndex] = dataSource;
        else
            definition.DataSources.Add( dataSource );

        ReportDefinitionHelper.EnsureDefinitionIds( definition );

        if ( resolveDataSources is not null )
            await resolveDataSources( definition, false );
    }

    internal async Task RefreshDataSourceAsync( ReportDefinition definition, IReportDataSourceProviderRegistry providerRegistry, string dataSourceName )
    {
        ReportDataSourceDefinition dataSource = FindDataSource( definition, dataSourceName );

        if ( dataSource is null )
            return;

        IReportDataSourceProvider provider = providerRegistry?.FindProvider( dataSource.ProviderType );

        if ( provider is null )
            return;

        try
        {
            dataSource.Schema = await provider.GetSchemaAsync( dataSource );
        }
        catch
        {
        }

        if ( !IsInMemoryProvider( dataSource.ProviderType ) )
            dataSource.Data = null;
    }

    internal void DeleteDataSource( ReportDefinition definition, string dataSourceName )
    {
        ReportDataSourceDefinition dataSource = FindDataSource( definition, dataSourceName );

        if ( dataSource is not null )
            definition.DataSources.Remove( dataSource );
    }

    internal void SaveFormulaField( ReportDefinition definition, ReportFormulaFieldDefinition formulaField )
    {
        if ( definition is null || formulaField is null || string.IsNullOrWhiteSpace( formulaField.Name ) )
            return;

        if ( definition.FormulaFields is null )
            definition.FormulaFields = [];

        ReportFormulaFieldDefinition existingFormulaField = definition.FormulaFields.FirstOrDefault( field =>
            string.Equals( field.Name, formulaField.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingFormulaField is null )
        {
            formulaField.Name = formulaField.Name.Trim();
            definition.FormulaFields.Add( formulaField );
        }
        else
        {
            existingFormulaField.Formula = formulaField.Formula;
        }
    }

    internal bool RenameFormulaField( ReportDefinition definition, string oldName, string newName )
    {
        ReportFormulaFieldDefinition formulaField = FindFormulaField( definition, oldName );

        if ( formulaField is null || FindFormulaField( definition, newName ) is not null )
            return false;

        formulaField.Name = newName;
        ReportDataReferenceUpdater.ReplaceFormulaFieldReferences( definition, oldName, newName );

        return true;
    }

    internal bool DeleteFormulaField( ReportDefinition definition, string formulaFieldName )
    {
        ReportFormulaFieldDefinition formulaField = FindFormulaField( definition, formulaFieldName );

        if ( formulaField is null )
            return false;

        definition.FormulaFields.Remove( formulaField );

        return true;
    }

    internal ReportElementDefinition CreateFormulaFieldElement( ReportDefinition definition, int sectionIndex, string formulaFieldName, double y )
    {
        if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count || FindFormulaField( definition, formulaFieldName ) is null )
            return null;

        ReportElementDefinition element = new ReportFieldElementDefinition
        {
            Name = formulaFieldName,
            DataSource = ReportFormulaFieldResolver.DataSourceName,
            Field = formulaFieldName,
            X = 0,
            Y = y,
            Width = ReportDesignerConstants.DefaultDroppedFieldWidth,
            Height = ReportDesignerConstants.DefaultDroppedFieldHeight,
        };

        AddDataElement( definition.Sections[sectionIndex], element, y );

        return element;
    }

    internal ReportElementDefinition CreateFieldElement( ReportDefinition definition, int sectionIndex, string dataSourceName, string fieldName, double y )
    {
        if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return null;

        ReportSectionDefinition section = definition.Sections[sectionIndex];
        (string DataSourceName, string FieldName) fieldBinding = ReportDefinitionHelper.NormalizeFieldBindingForSection( definition, section, dataSourceName, fieldName );
        ReportElementDefinition element = new ReportFieldElementDefinition
        {
            Name = fieldBinding.FieldName,
            Field = fieldBinding.FieldName,
            DataSource = fieldBinding.DataSourceName,
            X = 0,
            Y = y,
            Width = ReportDesignerConstants.DefaultDroppedFieldWidth,
            Height = ReportDesignerConstants.DefaultDroppedFieldHeight,
        };

        section.Elements.Add( element );

        if ( !ReportSpecialFieldResolver.IsSpecialDataSource( fieldBinding.DataSourceName )
            && !ReportFormulaFieldResolver.IsFormulaDataSource( fieldBinding.DataSourceName )
            && !ReportRunningTotalResolver.IsRunningTotalDataSource( fieldBinding.DataSourceName ) )
            ReportDetailHeaderSynchronizer.AddPageHeaderForDetailField( definition, sectionIndex, section, fieldBinding.FieldName, element.X, element.Width );

        section.Height = Math.Max( section.Height, y + ReportDesignerConstants.DefaultDroppedFieldHeight );

        return element;
    }

    internal void SaveRunningTotal( ReportDefinition definition, ReportRunningTotalDefinition runningTotal )
    {
        if ( definition is null || runningTotal is null || string.IsNullOrWhiteSpace( runningTotal.Name ) )
            return;

        if ( definition.RunningTotals is null )
            definition.RunningTotals = [];

        runningTotal.Name = runningTotal.Name.Trim();

        ReportRunningTotalDefinition existingRunningTotal = definition.RunningTotals.FirstOrDefault( field =>
            string.Equals( field.Id, runningTotal.Id, StringComparison.Ordinal )
            || string.Equals( field.Name, runningTotal.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingRunningTotal is null )
        {
            definition.RunningTotals.Add( runningTotal );
        }
        else
        {
            existingRunningTotal.Name = runningTotal.Name;
            existingRunningTotal.DataSource = runningTotal.DataSource;
            existingRunningTotal.Field = runningTotal.Field;
            existingRunningTotal.AggregateFunction = runningTotal.AggregateFunction;
            existingRunningTotal.EvaluateMode = runningTotal.EvaluateMode;
            existingRunningTotal.EvaluateFormula = runningTotal.EvaluateFormula;
            existingRunningTotal.ResetMode = runningTotal.ResetMode;
            existingRunningTotal.ResetGroupId = runningTotal.ResetGroupId;
        }
    }

    internal bool RenameRunningTotal( ReportDefinition definition, string oldName, string newName )
    {
        ReportRunningTotalDefinition runningTotal = FindRunningTotal( definition, oldName );

        if ( runningTotal is null || FindRunningTotal( definition, newName ) is not null )
            return false;

        runningTotal.Name = newName;
        ReportDataReferenceUpdater.ReplaceRunningTotalReferences( definition, oldName, newName );

        return true;
    }

    internal bool DeleteRunningTotal( ReportDefinition definition, string runningTotalName )
    {
        ReportRunningTotalDefinition runningTotal = FindRunningTotal( definition, runningTotalName );

        if ( runningTotal is null )
            return false;

        definition.RunningTotals.Remove( runningTotal );

        return true;
    }

    internal ReportElementDefinition CreateRunningTotalElement( ReportDefinition definition, int sectionIndex, string runningTotalName, double y )
    {
        if ( sectionIndex < 0 || sectionIndex >= definition.Sections.Count || FindRunningTotal( definition, runningTotalName ) is null )
            return null;

        ReportElementDefinition element = new ReportFieldElementDefinition
        {
            Name = runningTotalName,
            DataSource = ReportRunningTotalResolver.DataSourceName,
            Field = runningTotalName,
            X = 0,
            Y = y,
            Width = ReportDesignerConstants.DefaultDroppedFieldWidth,
            Height = ReportDesignerConstants.DefaultDroppedFieldHeight,
            Font = new()
            {
                Bold = true,
            },
        };

        AddDataElement( definition.Sections[sectionIndex], element, y );

        return element;
    }

    internal ReportFormulaFieldDefinition FindFormulaField( ReportDefinition definition, string formulaFieldName )
    {
        if ( definition?.FormulaFields is null || string.IsNullOrWhiteSpace( formulaFieldName ) )
            return null;

        string normalizedFormulaFieldName = ReportFormulaFieldResolver.NormalizeFieldName( formulaFieldName );

        return definition.FormulaFields.FirstOrDefault( field =>
            string.Equals( field.Name, normalizedFormulaFieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    internal ReportRunningTotalDefinition FindRunningTotal( ReportDefinition definition, string runningTotalName )
    {
        return ReportRunningTotalResolver.FindRunningTotal( definition, runningTotalName );
    }

    internal ReportDataSourceDefinition FindDataSource( ReportDefinition definition, string dataSourceName )
    {
        if ( definition?.DataSources is null || string.IsNullOrWhiteSpace( dataSourceName ) )
            return null;

        return definition.DataSources.FirstOrDefault( dataSource =>
            string.Equals( dataSource.Name, dataSourceName, StringComparison.OrdinalIgnoreCase ) );
    }

    private static void AddDataElement( ReportSectionDefinition section, ReportElementDefinition element, double y )
    {
        section.Elements.Add( element );
        section.Height = Math.Max( section.Height, y + ReportDesignerConstants.DefaultDroppedFieldHeight );
    }

    private static bool IsInMemoryProvider( string providerType )
    {
        return string.Equals( providerType, ObjectReportDataSourceProvider.ProviderType, StringComparison.OrdinalIgnoreCase )
            || string.Equals( providerType, DataSetReportDataSourceProvider.ProviderType, StringComparison.OrdinalIgnoreCase );
    }

    #endregion
}