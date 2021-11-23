#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// ICloseableComponent Manager
    /// </summary>
    public class CloseableAdapter
    {
        private readonly ICloseableComponent component;

        /// <summary>
        /// Default constructor for <see cref="CloseableAdapter"/>.
        /// </summary>
        /// <param name="component">Reference to the close activator.</param>
        public CloseableAdapter( ICloseableComponent component )
        {
            this.component = component;
        }

        /// <summary>
        /// Runs ICloseableComponent
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        public async Task Run( bool visible )
        {
            if ( component is IAnimatedComponent animatedComponent && animatedComponent.IsAnimated )
            {
                await animatedComponent.BeforeAnimation( visible );
                
                await Task.Delay( animatedComponent.AnimationDuration );
                
                if ( visible )
                    await animatedComponent.Open();
                else
                    await animatedComponent.Close();

                await animatedComponent.AfterAnimation( visible );
            }
            else
            {
                if ( visible )
                    await component.Open();
                else
                    await component.Close();
            }
        }
    }
}
