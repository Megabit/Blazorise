## Build Process

The Fluent UI icon list is generated from the embedded CSS font catalog. The regular and filled SVG sprites use the corresponding 20px assets downloaded from Microsoft's official `microsoft/fluentui-system-icons` GitHub repository. Keeping the CSS catalog as the source of supported names ensures that the font and SVG modes expose the same icons.

There are two ways to build the icons by running the `GetIcons.csx` script:

## 1. Using built-in C# scripting support in IDEs:
  - In Visual Studio or Rider using C# Interactive
  - In VS Code
  - Using [dotnet-repl](https://github.com/jonsequitur/dotnet-repl):

    ```bash
    dotnet repl --run GetIcons.csx
    ```

## 2. Install `dotnet-script`

Go to https://github.com/dotnet-script/dotnet-script and follow the instructions to install `dotnet-script` for your platform.

```bash
dotnet tool install -g dotnet-script
```

Then run the script:

```bash
dotnet script GetIcons.csx
```

---

- Check `FluentUIIcons.cs`, `wwwroot/fluentui-icons-regular.svg`, and `wwwroot/fluentui-icons-filled.svg` for changes.

## Update Process

- Run `GetIcons.csx`.
- Update the font version in the generated `FluentUIIcons.cs` header when the embedded CSS font assets change.
- Check the SVG sprite headers if the upstream package metadata changes.