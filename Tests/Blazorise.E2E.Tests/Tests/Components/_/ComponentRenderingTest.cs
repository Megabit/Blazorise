using Blazorise.E2E.Tests.Infrastructure;

namespace Blazorise.E2E.Tests.Tests.Components._
{
    [Parallelizable( ParallelScope.Self )]
    [TestFixture]
    public class ComponentRenderingTest : BlazorisePageTest
    {
        [Test]
        public async Task BasicTestAppCanBeServed()
        {
            await Page.GotoAsync( RootUri.AbsoluteUri );

            await Expect( Page ).ToHaveTitleAsync( "Blazorise test app" );
        }

        [Test]
        public async Task CanRenderTextOnlyComponent()
        {
            await Page.GotoAsync( RootUri.AbsoluteUri );

            //await Expect( Page ).ToHaveTitleAsync( "Blazorise test app" );
        }
    }
}
