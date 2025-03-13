## Run command

Run the following:

```
tailwindcss -i tailwind-build/blazorise.tailwind.prebuild.css -o wwwroot/blazorise.tailwind.min.css --minify
```

from the root of this project (not from this directory)

## Description

- `blazorise.tailwind.css` - blazorise custom classes, that need to be in the output.
- `blazorise.tailwind.prebuild.css` - the tw v4 css, contains all the imports and custom utilities 
- `btw-colors.css` - blazorise tailwind colors, 3 of them are actually used in the blazorise codebase
- `safelist.txt` - classes that need to be part of the output file. Mainly gathered from the TailwindClassProvider.