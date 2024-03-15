#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Dropdown : Blazorise.Dropdown
{
    #region Members

    private DomElement elementInfo;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        ParentAddon?.Register( this );

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        elementInfo = await JSUtilitiesModule.GetElementInfo( ElementRef, ElementId );

        await base.OnAfterRenderAsync( firstRender );
    }

    #endregion

    #region Properties

    public DomElement ElementInfo => elementInfo;

    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    [CascadingParameter] public AntDesign.Addon ParentAddon { get; set; }

    #endregion
}