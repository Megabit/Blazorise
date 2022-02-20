#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// Dynamically builds ListView component and it's items based in the supplied data.
    /// </summary>
    public partial class ListView<TItem> : ComponentBase
    {
        #region Methods

        protected Task SelectedListGroupItemChanged( string text )
        {
            SelectedItem = GetItemBySelectedText( text );

            return SelectedItemChanged.InvokeAsync( SelectedItem );
        }

        private TItem GetItemBySelectedText( string selectedText )
        {
            if ( Data is not null && TextField is not null )
            {
                return Data.FirstOrDefault( x => TextField.Invoke( x ) == selectedText );
            }

            return default;
        }

        protected string ListGroupClassNames
            => $"b-list-view {Class}";

        protected string ListGroupStyleNames
        {
            get
            {
                var sb = new StringBuilder();

                if ( !string.IsNullOrWhiteSpace( Height ) )
                    sb.Append( $"height:{Height};" );
                if ( !string.IsNullOrWhiteSpace( MaxHeight ) )
                    sb.Append( $"max-height:{MaxHeight};" );
                if ( !string.IsNullOrWhiteSpace( Style ) )
                    sb.Append( Style );

                return sb.ToString();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the list-group behavior mode.
        /// </summary>
        [Parameter]
        public ListGroupMode Mode { get; set; }

        /// <summary>
        /// Remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).
        /// </summary>
        [Parameter]
        public bool Flush { get; set; }

        /// <summary>
        /// Gets or sets the items data-source.
        /// </summary>
        [EditorRequired]
        [Parameter] public IEnumerable<TItem> Data { get; set; }

        /// <summary>
        /// Method used to get the display field from the supplied data source.
        /// </summary>
        [EditorRequired]
        [Parameter] public Func<TItem, string> TextField { get; set; }

        /// <summary>
        /// Currently selected item.
        /// </summary>
        [Parameter] public TItem SelectedItem { get; set; }

        /// <summary>
        /// Occurs after the selected item has changed.
        /// </summary>
        [Parameter] public EventCallback<TItem> SelectedItemChanged { get; set; }

        /// <summary>
        /// Custom css class-names.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom styles.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Sets the ListView Height. 
        /// Defaults to empty.
        /// </summary>
        [Parameter] public string Height { get; set; }

        /// <summary>
        /// Sets the ListView MaxHeight. 
        /// Defaults to 250px.
        /// </summary>
        [Parameter] public string MaxHeight { get; set; } = "250px";

        /// <summary>
        /// Captures all the custom attribute that are not part of Blazorise component.
        /// </summary>
        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="ListView{TItem}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
