## Build Process

The Ant Design icon list and inline SVG table are generated from the Ant Design icons GitHub repository.

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

- Check `AntDesignIcons.cs` and `AntDesignIconSvg.cs` for changes.

## Update Process

- Run `GetIcons.csx`.
- Check generated file headers if the package version changes.