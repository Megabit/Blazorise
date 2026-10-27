---
name: blazorise-e2e-tests
description: Create, review, or fix Blazorise Playwright E2E tests and their Razor test-app examples, including timing failures on CI. Use for Tests/Blazorise.E2E.Tests and related BasicTestApp fixtures, not ordinary bUnit tests or unrelated application development.
---

# Blazorise E2E tests

Follow [AGENTS.md](../../../AGENTS.md), including its execution policy. This skill does not authorize builds, tests, restores, dependency installation, or changes to core components. Reuse existing authorization for the same scope; request additional authorization only when the required execution exceeds it.

## Start from the real application path

- Read the test, its Razor fixture, and the shared navigation and hosting helpers. Compare established input and validation tests before inventing another setup pattern. The [E2E README](../../../Tests/Blazorise.E2E.Tests/ReadMe.md) describes the test host.
- Write fixtures as ordinary Razor markup using public Blazorise components, parameters, binding, and callbacks. Expose observable results through normal application state, such as the bound value, validation message, final item order, or callback count.
- Do not derive a replacement component, override initialization or animation, invoke internal handlers, or add a test-only readiness API to make an E2E test pass. If the evidence points to a product defect, explain it and respect the requested implementation scope.
- Use native Playwright clicks, keys, and mouse dragging. Do not replace a user interaction with synthetic DOM events, direct value assignments, or forced component state.
- Check the actual provider dependencies. Blazorise's Bootstrap extension stylesheet does not include Bootstrap itself. Missing base CSS can leave flex and padding classes inactive and empty drop zones without usable dimensions. Use real dependencies and component utilities instead of recreating their CSS in the fixture. Do not introduce a CDN dependency as an automatic fix; consider existing asset conventions and CI network availability.

## Synchronize with behavior, not elapsed time

An element being present, editable, or clickable does not prove that its asynchronous JavaScript initialization has completed. A click may make a race less frequent without fixing it. Likewise, a toggle button's new label proves that the UI rendered, not necessarily that the component finished applying its options.

Wait for an observable condition that follows the work being tested. Verify that ordering in the component lifecycle rather than guessing. Do not retry the action under assertion until it happens to succeed: repeated typing or dropping can hide lost input and change callback counts.

For an editor example that should initially receive focus, the public `Autofocus` option can supply a useful condition. The current MemoInput implementation initializes editing before applying autofocus. Wait for `ToBeFocusedAsync()` before a test click or keypress can focus the field itself. This is a specific example, not a requirement to add autofocus to every fixture. Recheck lifecycle ordering if the implementation changes.

Use Playwright's retrying assertions where possible. For layout measurements, prefer `BoundingBoxAsync()` and bounded C# polling that returns immediately on success. A short polling interval is different from an unconditional sleep. Match the ordinary assertion timeout, currently five seconds, unless a particular operation justifies more time. Report the expected change and last measurement on timeout.

Do not lengthen timeouts, add permanent sleeps, disable animation, force a browser feature fallback, or apply retries to the whole test merely because CI is slower.

## Observe layout without changing it

- Prefer computed styles, bounding boxes, and visible results over incidental inline `style` strings. Native CSS sizing may work correctly without the inline attribute a test expects.
- Capture baseline geometry only after layout settles. Coordinates measured before a render or animation can be stale by the next mouse action.
- A placeholder with `visibility: hidden` can still reserve space. Do not make it visible just to measure it. Check for a non-null bounding box and verify the behavior in the browser being tested; `display: none` has no usable layout box.
- Prefer named box properties (`X`, `Y`, `Width`, `Height`) over JavaScript arrays of coordinates. Use read-only JavaScript only when the required observation is unavailable through Playwright APIs, not to change application behavior.
- If JavaScript arguments are necessary, check serialization as well as browser behavior. The Playwright .NET serializer used during the MemoInput investigation did not serialize `float` as a JavaScript number; use a supported numeric type such as `double`. A timeout despite a visibly changed height may be a helper defect.

## Complete native drag and drop after layout changes

Treat drag start, insertion-point selection, and native drop acceptance as separate stages. An insertion placeholder proves that reordering state changed; it does not prove that releasing the mouse will produce a browser `drop` event.

During animation, the element under a stationary pointer can change. Chromium can then receive `dragenter` on a new target without an accepted `dragover` and cancel the drag on release. This produced unchanged item order and no dropped callback even though the correct placeholder had appeared.

Before release, follow the intended item after it settles and send a fresh native drag-over on the browser's current target. Recheck the expected insertion slot, then release the mouse. For an empty zone, use the zone itself. The current [DragDrop tests](../../../Tests/Blazorise.E2E.Tests/Tests/Components/DragDrop/DragDropAnimationTests.cs) demonstrate this in `MoveOver` and `CompleteDrop`.

Avoid both unsuccessful shortcuts: releasing immediately because the placeholder exists, and moving to previously captured placeholder coordinates that may now point at another item. Locator stability checks and current geometry matter more than an arbitrary number of hover calls.

Assert the final order and dropped callback, including the destination/index and count where relevant. For cancellation, assert restored items and no callback. Keep animated, non-animated, visible/hidden-placeholder, empty-zone, and reduced-motion paths in mind when changing a shared helper; run the affected coverage within the authorized scope.

## Diagnose CI failures with evidence

Read the current workflow, package versions, and runsettings before reproducing. Compare OS, SDK, build configuration, browser version/channel, headless mode, and parallelism. The local `.runsettings` has selected Edge while CI has used default bundled Chromium without that settings file. Windows/Edge passing does not establish Linux/Chromium behavior, and an `act` container or self-hosted runner is not automatically identical to a GitHub-hosted image.

Locate the exact failed assertion. A missing slot before movement, a slot disappearing after movement, and unchanged order after release are different failures. Measure the actual state rather than inferring that all three need another wait.

When needed, use temporary console observations, script-loading delays, CPU throttling, or a pause before release to test a specific hypothesis. Keep the real component behavior intact. For drag failures, observing `dragenter`, `dragover`, `drop`, `dragend`, their targets, and the application callback can distinguish a canceled native drop from an incorrect reorder.

Prefer a reproduction that fails before the fix and passes afterward under the same condition. Repeated local passes alone are not proof that a CI race is fixed. State clearly when the original failure was not reproduced or the target environment was unavailable.

Remove temporary logging, delays, throttling, repetitions, event observers, and trace hooks before finishing unless the user requested permanent diagnostics. Verify the cleaned-up implementation with the authorized focused tests. If a shared helper changes, assess its other callers rather than checking only the originally failing case.

## Keep verification and merge repairs focused

- Build only what changed. Reuse `--no-build` when the outputs are current. When only the test project changed and its referenced projects are already built and unchanged, `-p:BuildProjectReferences=false` can avoid rebuilding the full dependency graph. Do not use it to skip rebuilding changed Razor fixtures or library assets.
- Do not add builds, installations, or broad test runs without the authorization required by AGENTS.md. Once the relevant checks pass, expand testing only for a concrete remaining concern.
- After conflict resolution, search for stale helper calls and compare affected files with the intended PR and merge parents. Do not resurrect an obsolete helper just to satisfy the compiler if the new fixture deliberately replaced its behavior.
- Report what changed, why it addresses the observed failure, exactly which cases/environment were verified, and any remaining uncertainty. Preserve user changes and keep test-only fixes out of core code unless core changes are authorized.