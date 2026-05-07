# Blazorise.Animate

Blazorise.Animate is backed by Motion.

The public Blazor API remains compatible with previous releases. Existing animation names, easing names, duration and delay parameters, viewport options, and manual `Run()` usage are mapped to runtime options by the `Animate` component and passed through JavaScript interop.

## Runtime Integration

The vendored Motion browser bundle lives in `wwwroot/motion.js`. The Blazorise wrapper in `wwwroot/animate.js` is imported by the `Animate` component as a JavaScript module and loads Motion internally by injecting `_content/Blazorise.Animate/motion.js` when the first animation is requested.

The runtime exports the module functions used by the Blazor component:

- `init()` returns whether the runtime is available;
- `animate( element, options )` starts or observes an animation for the rendered element;
- `refresh()` remains available for compatibility;
- `dispose( element )` cancels any animation and observer owned by the element.

The Blazor component owns the runtime state and passes a typed options object to the JavaScript wrapper.

`wwwroot/blazorise.animate.js` and its generated variants are obsolete compatibility shims. They only print a console warning. New code must use the Blazor component, which imports `animate.js` automatically.

## Compatibility

The runtime keeps the existing Blazorise animation vocabulary:

- fade, slide, flip, and zoom animation names from `Animations`;
- easing names from `Easings`;
- `Duration`, `DurationMilliseconds`, `Delay`, and `DelayMilliseconds`;
- `Mirror`, `Once`, `Offset`, `Anchor`, and `AnchorPlacement`;
- automatic and manual execution through `Auto` and `Run()`.

The default trigger is viewport based to preserve existing scroll-reveal behavior. `AnimationTrigger.Render` can be used for general component animations that should run immediately after render.

`Visible` can be used for enter and leave animations without wrapping the component in an `@if`. When `Visible` changes to `false`, the component keeps its content rendered until the exit animation completes. `AnimateOnInitialRender` can be set to `false` when content should appear immediately on page load and only animate on later visibility changes.

`AnimatedSize` can be set to `AnimatedSize.Width` or `AnimatedSize.Height` when the element should animate its occupied size as part of a visibility transition. This is intended for components such as sidebars, drawers, or vertical disclosure panels where a transform-only animation would visually move the content while still reserving its original space.

## Custom Animations

`IAnimation` remains the stable animation contract. Existing implementations only need to provide `Name`.

For animations that should carry their own runtime keyframes, implement `IAnimationDefinition` or use `AnimationDefinition`:

```csharp
public static class AppAnimations
{
    public static IAnimation SoftEnter => new AnimationDefinition(
        "soft-enter",
        new[]
        {
            new AnimationFrame { Opacity = 0, Y = "1rem", Scale = 0.98 },
            new AnimationFrame { Opacity = 1, Y = "0", Scale = 1 },
        } );
}
```

The `Animate` component sends structured keyframes to the JavaScript module when available. If keyframes are missing or invalid, the runtime falls back to a simple fade animation.

## Motion Notes

The wrapper uses Motion's `animate()` API for element animations and Motion's `inView()` API for viewport-triggered animations. Application code should continue to reference only Blazorise's `Animate` component.

Motion is vendored from the `motion` npm package. The current vendored version is `12.38.0`; its MIT license is copied to `MOTION_LICENSE.md`.

## Updating

When changing the runtime:

1. Update `wwwroot/animate.js`.
2. Update `wwwroot/motion.js` only from an official `motion` npm package release.
3. Update `MOTION_LICENSE.md` if the vendored package license changes.
4. Keep `wwwroot/blazorise.animate.js` as an obsolete compatibility shim unless the migration policy changes.
5. Keep generated documentation snippet files untouched.
6. Manually verify initial page load, route navigation, dynamically rendered `Animate` components, removed and reinserted components, viewport-triggered animations, and `AnimationTrigger.Render`.