## Build Process

- Run the `GetIcons.csx` script:
  - In Visual Studio or Rider using C# Interactive
  - In VS Code
  - Using [dotnet-repl](https://github.com/jonsequitur/dotnet-repl):

    ```bash
    dotnet repl --run GetIcons.csx 
    ```

- Check `FontAwesomeIcons.cs` for changes (there should be none unless you modified the input script)

## Update Process

- Update the `url` inside the `.csx` script to grab the new version (see the comment in there)
- Update the version number in the script header