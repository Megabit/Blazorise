﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign
{
    public partial class Carousel : Blazorise.Carousel
    {
        #region Members

        private ElementReference slickListElementRef;

        private int slickWidth = -1;

        private int totalWidth = -1;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            base.BuildClasses( builder );
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            var listRect = await JSRunner.GetElementInfo( slickListElementRef, null );

            if ( slickWidth != (int)listRect.BoundingClientRect.Width )
            {
                slickWidth = (int)listRect.BoundingClientRect.Width;
                totalWidth = slickWidth * ( carouselSlides.Count * 2 + 1 );

                StateHasChanged();
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        #endregion

        #region Properties

        protected string SlickTrackStyle
        {
            get
            {
                var slideIndex = carouselSlides.IndexOf( carouselSlides.FirstOrDefault( x => x.Name == SelectedSlide ) );

                return $"width: {totalWidth}px; opacity: 1; transform: translate3d(-{slickWidth * ( slideIndex + 1 )}px, 0px, 0px);transition: -webkit-transform 500ms ease 0s;";
            }
        }

        protected string SlickClonedStyle
            => $"width: {slickWidth}px;";

        protected string SlickStyle
            => $"outline: none; width: {slickWidth}px;";

        #endregion
    }
}
