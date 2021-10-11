using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.Components.AutoComplete
{
    public class ItemContext<TItem, TValue>
    {

        public ItemContext( TItem item, TValue value, string text )
        {
            Item = item;
            Value = value;
            Text = text;
        }
        public TItem Item { get;  }

        public TValue Value { get; }

        public string Text { get; }

    }
}
