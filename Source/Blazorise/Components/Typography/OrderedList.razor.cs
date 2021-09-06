﻿#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An ordered list created using the &lt;ul&gt; element.
    /// </summary>
    public partial class OrderedList : BaseTypographyComponent
    {
        #region Members

        private bool unstyled;

        private OrderedListType listType = OrderedListType.Default;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.OrderedList() );
            builder.Append( ClassProvider.OrderedListUnstyled( Unstyled ) );
            builder.Append( ClassProvider.OrderedListType( ListType ), ListType != OrderedListType.Default );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Remove the default <c>list-style</c> and left margin on list items (immediate children only).
        /// </summary>
        [Parameter]
        public bool Unstyled
        {
            get => unstyled;
            set
            {
                unstyled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the type of item markers.
        /// </summary>
        [Parameter]
        public OrderedListType ListType
        {
            get => listType;
            set
            {
                listType = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
