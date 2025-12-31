## Run command

From `Source/Blazorise.Tailwind` install the toolchain and build:

```
npm install
npm run build:css
```

For local development you can watch changes:

```
npm run watch:css
```

## Description

- `tailwind-build/blazorise.tailwind.css` - blazorise custom classes compiled from SCSS that need to be in the output.
- `tailwind-build/btw-prebuild.css` - the Tailwind v4 entry point; contains imports, theme overrides, content sources, Flowbite plugin, and custom utilities.
- `tailwind-build/btw-colors.css` - blazorise tailwind colors, 3 of them are actually used in the blazorise codebase.
- `tailwind-build/safelist.txt` - classes that need to be part of the output file. Mainly gathered from the TailwindClassProvider.
- `wwwroot/blazorise.tailwind.css` - unminified Tailwind output.
- `wwwroot/blazorise.tailwind.min.css` - minified Tailwind output.
