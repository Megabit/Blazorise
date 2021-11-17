#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The <c>abbr</c> tag defines an abbreviation or an acronym, like "HTML", "CSS", "Mr.", "Dr.", "ASAP", "ATM".
    /// </summary>
    public partial class Abbreviation : BaseElementComponent
    {
        #region Properties

        /// <summary>
        /// The title attribute specifies extra information about an element.
        /// </summary>
        [Parameter] public string Title { get; set; }

        #endregion
    }
}
