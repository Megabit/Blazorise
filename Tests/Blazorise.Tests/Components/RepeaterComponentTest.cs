using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class RepeaterComponentTest
    {
        private readonly EventCallbackFactory callbackFactory = new();

        [Fact]
        public void ItemsNull_DoesNothing()
        {
            var ctx = new TestContext();

            var comp = ctx.RenderComponent<Repeater<int>>( builder => builder
                .Add( p => p.Items, default )
            );

            Assert.True( string.IsNullOrWhiteSpace( comp.Markup ) );
        }

        [Fact]
        public void Items_Render()
        {
            var ctx = new TestContext();

            var items = Enumerable.Range( 0, 10 ).ToList();

            var comp = ctx.RenderComponent<Repeater<int>>( builder => builder
                .Add( p => p.Items, items )
                .Add( p => p.ChildContent, x => x.ToString() )
            );

            Assert.Equal( string.Concat( items.Select( x => x.ToString() ) ), comp.Markup );
        }

        [Fact]
        public void CollectionChanged_Updates()
        {
            var ctx = new TestContext();

            var items = new ObservableCollection<int>( Enumerable.Range( 0, 10 ) );

            var watcher = new CollectionChangedWatcher();

            var comp = ctx.RenderComponent<Repeater<int>>( builder => builder
                .Add( p => p.Items, items )
                .Add( p => p.ChildContent, x => x.ToString() )
                .Add( p => p.CollectionChanged, callbackFactory.Create( watcher, (Action<NotifyCollectionChangedEventArgs>)watcher.OnCollectionChanged ) )
            );

            Assert.Equal( string.Concat( items.Select( x => x.ToString() ) ), comp.Markup );
            Assert.Equal( 1, watcher.Count );

            items.Add( 10 );

            Assert.Equal( string.Concat( items.Select( x => x.ToString() ) ), comp.Markup );
            Assert.Equal( 2, watcher.Count );

            items.Clear();

            Assert.True( string.IsNullOrWhiteSpace( comp.Markup ) );
            Assert.Equal( 3, watcher.Count );
        }

        [Fact]
        public void ReplaceCollection_Updates()
        {
            var ctx = new TestContext();

            var items = new ObservableCollection<int>( Enumerable.Range( 0, 10 ) );

            var watcher = new CollectionChangedWatcher();

            var comp = ctx.RenderComponent<Repeater<int>>( builder => builder
                .Add( p => p.Items, items )
                .Add( p => p.ChildContent, x => x.ToString() )
                .Add( p => p.CollectionChanged, callbackFactory.Create( watcher, (Action<NotifyCollectionChangedEventArgs>)watcher.OnCollectionChanged ) )
            );

            Assert.Equal( string.Concat( items.Select( x => x.ToString() ) ), comp.Markup );
            Assert.Equal( 1, watcher.Count );

            comp.SetParametersAndRender( builder => builder.Add( p => p.Items, items ) );

            Assert.Equal( string.Concat( items.Select( x => x.ToString() ) ), comp.Markup );
            Assert.Equal( 1, watcher.Count );

            var updated = Enumerable.Range( 10, 20 ).ToList();
            comp.SetParametersAndRender( builder => builder.Add( p => p.Items, updated ) );

            Assert.Equal( string.Concat( updated.Select( x => x.ToString() ) ), comp.Markup );
            Assert.Equal( 2, watcher.Count );
        }

        [Fact]
        public void ReplaceCollectionWithNull_Clears()
        {
            var ctx = new TestContext();

            var items = Enumerable.Range( 0, 10 ).ToList();

            var comp = ctx.RenderComponent<Repeater<int>>( builder => builder
                .Add( p => p.Items, items )
                .Add( p => p.ChildContent, x => x.ToString() )
            );

            Assert.Equal( string.Concat( items.Select( x => x.ToString() ) ), comp.Markup );

            comp.SetParametersAndRender( builder => builder.Add( p => p.Items, default ) );

            Assert.True( string.IsNullOrWhiteSpace( comp.Markup ) );
        }

        [Fact]
        public void TakeSkip_Renders()
        {
            var ctx = new TestContext();

            var items = Enumerable.Range( 0, 10 ).ToList();

            var comp = ctx.RenderComponent<Repeater<int>>( builder => builder
                .Add( p => p.Items, items )
                .Add( p => p.Take, 2 )
                .Add( p => p.Skip, 2 )
                .Add( p => p.ChildContent, x => x.ToString() )
            );

            Assert.Equal( string.Concat( items.Take( 2 ).Skip( 2 ).Select( x => x.ToString() ) ), comp.Markup );

            comp.SetParametersAndRender( builder => builder.Add( p => p.Take, default ) );

            Assert.Equal( string.Concat( items.Skip( 2 ).Select( x => x.ToString() ) ), comp.Markup );
        }

        private class CollectionChangedWatcher
        {
            public void OnCollectionChanged( NotifyCollectionChangedEventArgs ea )
            {
                Count++;
            }

            public int Count { get; set; }
        }
    }
}