using System;

namespace Blazorise.Reporting;

public enum ReportStudioMode
{
    Design,
    Preview
}

[Flags]
public enum ReportPreviewFormat
{
    Html = 1,
    Pdf = 2
}

public enum ReportDefinitionMode
{
    SeedWhenEmpty,
    AlwaysUseDeclarative,
    UseDefinitionOnly
}

public enum ReportLayout
{
    Absolute,
    Stack,
    Flex,
    Grid
}

public enum ReportPageSize
{
    A4,
    Letter
}

public enum ReportOrientation
{
    Portrait,
    Landscape
}

public enum ReportSectionType
{
    Header = 0,
    Detail = 1,
    Footer = 2,
    Group = 3,
    ReportHeader = Header,
    PageHeader = 4,
    GroupHeader = 5,
    GroupFooter = 6,
    PageFooter = 7,
    ReportFooter = Footer
}

public enum ReportElementType
{
    Text,
    Field,
    Table,
    Image,
    Line,
    Rectangle,
    PageBreak
}

public enum ReportCommand
{
    Design,
    Preview,
    PreviewHtml,
    PreviewPdf,
    Cut,
    Copy,
    Paste,
    Delete,
    Undo,
    Redo,
    Reset
}

public enum ReportSelectionType
{
    Report,
    Section,
    Element
}