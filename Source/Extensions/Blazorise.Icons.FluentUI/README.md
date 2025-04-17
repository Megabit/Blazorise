## Build Process

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

- Check `FluentUIIcons.cs` for changes (there should be none unless you modified the input script)

## Update Process

- Update the `url` inside the `.csx` script to grab the new version (see the comment in there)
- Update the version number in the script header