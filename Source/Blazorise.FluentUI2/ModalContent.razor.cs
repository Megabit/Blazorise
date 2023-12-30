using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.FluentUI2;

public partial class ModalContent
{
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( ParentModal is FluentUI2.Modal fluentModal
            && parameters.TryGetValue<ModalSize>( nameof( Size ), out var paramSize ) && paramSize != Size )
        {
            fluentModal.NotifyModalSizeChanged( paramSize );
        }

        return base.SetParametersAsync( parameters );
    }
}
