#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise
{
    public struct DomElement
    {
        public DomRectangle BoundingClientRect { get; set; }

        public double OffsetTop { get; set; }

        public double OffsetLeft { get; set; }

        public double OffsetWidth { get; set; }

        public double OffsetHeight { get; set; }

        public double ScrollTop { get; set; }

        public double ScrollLeft { get; set; }

        public double ScrollWidth { get; set; }

        public double ScrollHeight { get; set; }

        public double ClientTop { get; set; }

        public double ClientLeft { get; set; }

        public double ClientWidth { get; set; }

        public double ClientHeight { get; set; }
    }
}
