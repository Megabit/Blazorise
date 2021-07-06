using System.Threading.Tasks;
using Blazorise.Utilities;

namespace Blazorise.AntDesign
{
    public partial class Table
    {
        protected override void BuildClasses( ClassBuilder builder )
        {
            base.BuildClasses( builder );

            builder.Append( ClassProvider.TableFixedHeader(), FixedHeader );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            // don't add anything for antdesign
        }

        protected override ValueTask InitializeTableFixedHeader()
        {
            // antdesign has a different table structure so we don't need to do anything

            return ValueTask.CompletedTask;
        }
    }
}
