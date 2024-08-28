#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign;

public partial class Addon : Blazorise.Addon
{
    #region Members

    private bool hasDropdown;

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( hasDropdown )
            builder.Append( "ant-input-group-addon-dropdown" );

        base.BuildClasses( builder );
    }

    protected override async Task OnFirstAfterRenderAsync()
    {
        if ( hasDropdown )
        {
            DirtyClasses();

            await InvokeAsync( StateHasChanged );
        }

        await base.OnFirstAfterRenderAsync();
    }

    internal void Register( AntDesign.Components.Dropdown dropdown )
    {
        if ( dropdown != null )
            hasDropdown = true;
    }

    #endregion

    #region Properties

    #endregion
}