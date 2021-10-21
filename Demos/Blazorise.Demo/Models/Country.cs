using System.Collections.Generic;
using CountryData;

namespace Blazorise.Demo.Data
{
    public class Country : ICountryInfo
    {
        public string Name { get; }
        public string Iso { get; }
        public string Iso3 { get; }
        public ushort IsoNumeric { get; }
        public Fips? Fips { get; }
        public string Capital { get; }
        public double? Area { get; }
        public uint Population { get; }
        public string Continent { get; }
        public string TopLevelDomain { get; }
        public CurrencyCode? CurrencyCode { get; }
        public string CurrencyName { get; }
        public string PhonePrefix { get; }
        public string PostCodeFormat { get; }
        public string PostCodeRegex { get; }
        public IReadOnlyList<string> Languages { get; }
    }

}