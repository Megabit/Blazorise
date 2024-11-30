## Code Removal with Fody

Attributes like `GenerateIgnoreEqualityAttribute` are used to signal the source generator to produce additional code. These attributes have no functional purpose in the final compiled code, so they are removed post-compilation using Fody.

If you add a new attribute here, make sure to account for its removal in `Blazorise.Weavers.Fody`.

> It might be worthwhile to place "ready-to-be-removed" attributes in a dedicated namespace, making it easier to remove the entire namespace rather than individual attributes.

## ApiDocsDtos
- Is used by generated code (`ComponentApiDocsGenerator.cs`) and by `Blazorise.Docs` to present the Blazorise API.
- Is removed while packing the Blazorise nuget (using Fody).