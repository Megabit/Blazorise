#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CarouselSlide : BaseComponent
    {
        #region Members

        private CarouselStore parentCarouselStore;

        #endregion

        #region Constructors

        public CarouselSlide()
        {
            IndicatorClassBuilder = new ClassBuilder( BuildIndicatorClasses );
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselSlide() );
            builder.Append( ClassProvider.CarouselSlideActive( Active ) );

            base.BuildClasses( builder );
        }

        private void BuildIndicatorClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CarouselIndicator() );
            builder.Append( ClassProvider.CarouselIndicatorActive( Active ) );
        }

        protected internal override void DirtyClasses()
        {
            IndicatorClassBuilder.Dirty();

            base.DirtyClasses();
        }

        protected override void OnInitialized()
        {
            if ( ParentCarousel != null )
            {
                ParentCarousel.AddSlide( this );
            }

            base.OnInitialized();
        }

        public void Activate()
        {
            DirtyClasses();

            ParentCarousel.Select( Name );
        }

        #endregion

        #region Properties

        public bool Active => parentCarouselStore.CurrentSlide == Name;

        protected ClassBuilder IndicatorClassBuilder { get; private set; }

        public string IndicatorClassNames => IndicatorClassBuilder.Class;

        public string IndicatorStyleNames => null;

        /// <summary>
        /// Defines the slide name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        [CascadingParameter]
        protected CarouselStore ParentCarouselStore
        {
            get => parentCarouselStore;
            set
            {
                if ( parentCarouselStore == value )
                    return;

                parentCarouselStore = value;

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Carousel ParentCarousel { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
