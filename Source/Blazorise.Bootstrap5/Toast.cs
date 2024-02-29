using System.Threading.Tasks;

namespace Blazorise.Bootstrap5;

public class Toast : Blazorise.Toast
{
    protected override async Task SetVisibleState( bool visible )
    {
        if ( visible )
            State = State with { Visible = visible };

        await HandleVisibilityStyles( visible );
        await RaiseEvents( visible );

        if ( !visible )
            State = State with { Visible = visible };
    }
}
