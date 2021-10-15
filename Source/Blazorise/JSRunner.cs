#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class JSRunner : IJSRunner
    {
        #region Members

        private readonly IJSRuntime runtime;

        private const string BLAZORISE_NAMESPACE = "blazorise";

        #endregion

        #region Constructors

        public JSRunner( IJSRuntime runtime )
        {
            this.runtime = runtime;
        }

        #endregion

        #region Methods

        #region Select

       
        #endregion

        #endregion

        #region Properties

        protected IJSRuntime Runtime => runtime;

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
