#region Using directives
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion


namespace Blazorise.Docs.Layouts;

public partial class MainLayout
{
    #region Members

    private bool topbarVisible = false;

    #endregion

    #region Properties

    private static IFluentBorder SelectedBorders = Border.Rounded.Is1.Primary.OnBottom.Is1.Primary.OnTop;

    private IFluentBorder CommunityBorders => !IsComercialPage ? SelectedBorders : null;
    private TextWeight CommunityTextWeight => !IsComercialPage ? TextWeight.Normal : TextWeight.Default;
    private Shadow CommunityShadow => !IsComercialPage ? Shadow.Small : Shadow.None;

    private IFluentBorder CommercialBorders => IsComercialPage ? SelectedBorders : null;
    private TextWeight CommercialTextWeight => IsComercialPage ? TextWeight.Normal : TextWeight.Default;
    private Shadow CommercialShadow => IsComercialPage ? Shadow.Small : Shadow.None;

    private bool IsComercialPage => NavigationManager.Uri != null && NavigationManager.ToBaseRelativePath( NavigationManager.Uri ) == "commercial";

    [Inject] public NavigationManager NavigationManager { get; set; }

    #endregion
}