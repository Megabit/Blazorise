## Build Process

### Clone FontAwesome

Clone the FontAwesome repository. Once downloaded make sure you are on the latest branch.

```bash
git clone https://github.com/FortAwesome/Font-Awesome.git
```

### Extract Names

To build the icon names from FontAwesome run the following from the console app.

Once it is finished open `icons_export.txt` and copy/paste it into the `FontAwesomeIcons.cs` file.

```cs
internal class Program
{
    static void Main( string[] args )
    {
        ExtractFontAwesomeNames();
    }

    static void ExtractFontAwesomeNames()
    {
        var sourceFile = @"<path to FontAwesome metadata folder>\icons.json";
        var outputFile = @"<path to FontAwesome metadata folder>\icons_export.txt";

        var values = JsonSerializer.Deserialize<Dictionary<string, FontAwesomeIcon>>(
            File.ReadAllText(sourceFile),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var resultsWithNames = (from v in values
                                select new
                                {
                                    DisplayName = GetDisplayName(v.Key),
                                    Key = $"fa-{v.Key}",
                                }).ToList();

        var resultsWithAliases = (from v in values
                                  where v.Value?.Aliases?.Names?.Count > 0
                                  from a in v.Value.Aliases.Names
                                  select new
                                  {
                                      DisplayName = GetDisplayName(a),
                                      Key = $"fa-{v.Key}",
                                  }).ToList();

        var result = resultsWithNames.Concat(resultsWithAliases).OrderBy(x => x.DisplayName).ToList();

        File.WriteAllLines(outputFile,
            result.Select(x => $"public const string {x.DisplayName} = \"{x.Key}\";"));
    }

    static string GetDisplayName(string value)
    {
        var pascalCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value).Replace("-", "");

        if (char.IsDigit(pascalCase.First()))
        {
            return $"_{pascalCase}";
        }

        return pascalCase;
    }

    public class FontAwesomeIcon
    {
        public FontAwesomeIconAliases Aliases { get; set; }
    }

    public class FontAwesomeIconAliases
    {
        public List<string> Names { get; set; }
    }
}
```