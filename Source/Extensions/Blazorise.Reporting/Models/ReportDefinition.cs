using System;
using System.Collections.Generic;

namespace Blazorise.Reporting;

public sealed class ReportDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    public string Name { get; set; }

    public ReportPageDefinition Page { get; set; } = new();

    public List<ReportDataSourceDefinition> DataSources { get; set; } = [];

    public List<ReportSectionDefinition> Sections { get; set; } = [];
}

public sealed class ReportPageDefinition
{
    public ReportPageSize Size { get; set; } = ReportPageSize.A4;

    public ReportOrientation Orientation { get; set; } = ReportOrientation.Portrait;

    public double Width { get; set; } = 794;

    public double Height { get; set; } = 1123;
}

public sealed class ReportDataSourceDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    public string Name { get; set; }

    public object Data { get; set; }
}

public sealed class ReportSectionDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    public string Name { get; set; }

    public ReportSectionType Type { get; set; }

    public ReportLayout Layout { get; set; } = ReportLayout.Absolute;

    public double Height { get; set; } = 80;

    public string DataSource { get; set; }

    public string Class { get; set; }

    public string Style { get; set; }

    public bool Default { get; set; }

    public bool Suppressed { get; set; }

    public List<ReportElementDefinition> Elements { get; set; } = [];
}

public sealed class ReportElementDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    public string Name { get; set; }

    public ReportElementType Type { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Width { get; set; } = 120;

    public double Height { get; set; } = 24;

    public string Text { get; set; }

    public string Field { get; set; }

    public string Format { get; set; }

    public string Source { get; set; }

    public string DataSource { get; set; }

    public string Class { get; set; }

    public string Style { get; set; }

    public List<ReportTableColumnDefinition> Columns { get; set; } = [];
}

public sealed class ReportTableColumnDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    public string Title { get; set; }

    public string Field { get; set; }

    public string Format { get; set; }

    public double Width { get; set; } = 120;
}