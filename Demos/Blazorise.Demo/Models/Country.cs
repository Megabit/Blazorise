using System.Collections.Generic;
using CountryData;

namespace Blazorise.Demo.Data
{
    public class Country 
    {

        public Country( string name, string iso, string capital )
        {
            Name = name;
            Iso = iso;
            Capital = capital;
        }
        public string Name { get;  }
        public string Iso { get; }
        public string Capital { get; }

    }

}