using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.FluentUI2.Components;

public partial class ModalContent
{
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( ParentModal is FluentUI2.Components.Modal fluentModal )
        {
            if ( parameters.TryGetValue<ModalSize>( nameof( Size ), out var paramSize ) && paramSize != Size )
                fluentModal.NotifyModalSizeChanged( paramSize );

            if ( parameters.TryGetValue<bool>( nameof( Centered ), out var paramCentered ) && paramCentered != Centered )
                fluentModal.NotifyModalCenteredChanged( paramCentered );
        }

        return base.SetParametersAsync( parameters );
    }
}
