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

## Class merging

The component class builder merges completed class lists when it rebuilds and caches the merged result. Reading `ClassNames` during rendering does not repeat the merge. Common conflicting utilities keep the last class at the same importance, so `px-5 py-2.5 p-2` becomes `p-2`, while `p-2 px-5` keeps both. Important utilities take precedence regardless of their position: `px-5 py-2.5 !py-3` becomes `px-5 !py-3`, and `!p-3 px-5 py-2.5` becomes `!p-3`. A shorthand is retained when it still supplies unaffected sides, so `p-2 !py-3` keeps both. Existing append order determines priority within the same importance: component defaults, `Class`, utility parameters, then typed custom classes.

The helper handles common spacing, sizing, flex and grid layout, visibility, alignment, object fitting, typography, whitespace, text overflow, translation, standard and Blazorise colors, borders, radii, rings, outlines, shadows, and common interaction and transition utilities. Unknown theme names and ambiguous arbitrary values are preserved. Explicitly typed arbitrary colors and lengths are supported, including opacity and line-height suffixes. Variant order is compared exactly, and different variant scopes are preserved. Important modifiers before variants (`!md:p-2`), before utilities (`md:!p-2`), and after utilities (`md:p-2!`) share the same merge scope. The helper does not inspect stylesheets or custom themes, and does not generate CSS.

Root component class builders and Tailwind-owned component-part builders use merging. Intermediate utility builders and shared component-part builders retain their existing behavior. All Tailwind conflict rules remain in this project; other CSS providers preserve the original class list.