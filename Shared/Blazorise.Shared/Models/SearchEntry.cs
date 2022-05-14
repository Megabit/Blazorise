using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.Shared.Models
{
    public class SearchEntry
    {
        public SearchEntry( string url, string name ) //maybe add subtitle, and use <ItemContent> in Autocomplete where this is displayed
        {
            Url = url;
            Name = name;
        }

        public string Name { get; }
        public string Url { get; }
    }
}
