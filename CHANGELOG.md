# Changelog
** This changelog as of now is maintained by AI and is unreviewed. Take this more as a summary than a real changelog. May be removed and properly taken care of once changes aren't so durastic. **
## 1.0.3

### Added

- Added `RunstarSystems.ECS.Order` assembly definition for subgroup ordering logic.
- Added `RunstarSystems.ECS.Core` as an assembly reference for the order assembly.
- Added shared `IECSGroupOrderAttribute` contract for fixed, update, and late group order attributes.
- Added generic subgroup registration path through `RegisterGroups<TAttribute>(...)`.
- Added shared ordered group discovery through `FindOrderedGroups<TAttribute>(...)`.
- Added shared ordered group insertion through `RegisterOrderedGroups(...)`.
- Added `OrderedGroupInfo` data struct for storing ordered group type and order metadata.
- Added partial class split for `SubGroupOrganizer` so registration flow and generic ordered-group helpers can live in separate files.

### Changed

- Changed `ECSFixedGroupOrderAttribute`, `ECSUpdateGroupOrderAttribute`, and `ECSLateGroupOrderAttribute` to share a common order interface.
- Changed group order attribute naming toward C# convention by using `Order` instead of the raw `order` field.
- Changed subgroup registration so fixed, update, and late groups use the same templated registration path.
- Changed `SubGroupOrganizer` so the main organizer file owns the pipeline/root setup, `AddGroupToParent(...)`, and `CompareOrderedGroups(...)`.
- Changed the partial subgroup helper file to own `RegisterGroups<TAttribute>(...)`, `FindOrderedGroups<TAttribute>(...)`, and `RegisterOrderedGroups(...)`.
- Changed subgroup order logic so duplicated `RegisterFixedGroups(...)`, `RegisterUpdateGroups(...)`, and `RegisterLateGroups(...)` functions are no longer needed.
- Changed order functionality to live in its own assembly while still remaining inside the same `com.runstarsystems.ecs` package folder structure.

### Removed

- Removed duplicate fixed/update/late subgroup registration implementations.
- Removed separate `GetFixedOrder(...)`, `GetUpdateOrder(...)`, and `GetLateOrder(...)` helpers.
- Removed the need for each subgroup registration path to manually pass a custom order getter.
- Removed the assumption that package folder location determines assembly dependency behavior.

### Fixed

- Fixed subgroup organizer complexity by moving repeated ordered-group logic into one templated path.
- Fixed assembly definition confusion by making `RunstarSystems.ECS.Order` explicitly reference `RunstarSystems.ECS.Core`.
- Fixed dependency direction so order logic depends on core infrastructure without requiring core to depend on order logic.
- Fixed naming clarity between Unity package folders, assembly definition names, and C# namespaces.

### Notes

- `RunstarSystems.ECS.Order.asmdef` should reference `RunstarSystems.ECS.Core`.
- Multiple assemblies can still share the `RunstarSystems.ECS` root namespace.
- The order assembly can live inside the same `com.runstarsystems.ecs` package while remaining a separate compile unit.
- `SubGroupOrganizer` should remain a partial class so the main organizer flow stays small.
- `OrderedGroupInfo` should remain in the data namespace as shared metadata for ordered subgroup registration.

## 1.0.2

### Added

- Added `com.runstarsystems.order` as a package dependency for shared ordering logic.
- Added support for separating package dependency names, assembly definition names, and C# namespaces.
- Added expected order assembly naming through `RunstarSystems.ECS.Order`.
- Added clearer Unity project manifest guidance for GitHub-based package installation.
- Added registry metadata support for inherited and direct matches.
- Added metadata-aware registry query paths through `GetMatches(...)`.
- Added type-only registry query guidance through `Get<TAttribute>()`.
- Added inheritance-focused registry behavior where resolved inherited metadata is added back into the registry.
- Added `InheritCache` as a separate lookup cache for parent/source type to inheritable metadata.
- Added support for treating the `TypeRegistry` as the effective metadata source after inheritance has resolved.

### Changed

- Split registry responsibilities so `TypeRegistry` owns global attribute-to-metadata storage while `InheritCache` owns parent/source-to-inheritable lookup caching.
- Changed inheritance behavior so later organizers can query the registry normally instead of using reflection or reading temporary inheritance tables.
- Changed registry usage expectations:
  - Use `Get<TAttribute>()` when only the matched type list is needed.
  - Use `GetMatches(...)` when metadata values, source type, or inherited/direct state are needed.
- Changed package setup so `com.runstarsystems.ecs` depends on `com.runstarsystems.order` by version in its package manifest.
- Changed GitHub package workflow so consuming Unity projects should install both `com.runstarsystems.order` and `com.runstarsystems.ecs` directly when no scoped registry is available.
- Changed assembly reference expectations so `.asmdef` files reference assembly names, not package names or root namespaces.
- Changed namespace expectations so multiple assemblies can share the same `RunstarSystems.ECS` namespace without implying assembly references.
- Changed priority subgroup/order logic so it is no longer core infrastructure and can live in its own package.
- Changed package structure direction so priority subgroup ordering may come back as its own assembly definition while still living in the project when that makes dependency management easier.
- Changed the core package direction so inheritance and registry infrastructure stay in core, while group ordering can remain a consumer of that infrastructure.

### Removed

- Removed priority subgroup ordering from the core dependency path.
- Removed the assumption that priority group ordering must be bundled directly into the main ECS core assembly.
- Removed the assumption that `using RunstarSystems.ECS` can automatically import child namespaces.
- Removed the assumption that C# supports wildcard namespace imports such as `RunstarSystems.ECS.*`.
- Removed the assumption that package Git dependencies are automatically resolved through another Git package's `package.json`.

### Fixed

- Fixed confusion between Unity package names, assembly names, root namespaces, and C# namespaces.
- Fixed dependency setup confusion where `com.runstarsystems.ecs` could fail to install because Unity could not locate `com.runstarsystems.order`.
- Fixed registry query confusion around inherited metadata.
- Fixed the difference between type-only registry queries and metadata-aware registry queries.
- Fixed overlap concerns between `TypeRegistry` and `InheritCache` by making their responsibilities separate:
  - `TypeRegistry` stores all effective metadata.
  - `InheritCache` stores cached inheritable metadata by source type.
- Fixed the expectation that inherited metadata must be found through reflection.
- Fixed the expectation that attributes can be physically added to child types at runtime.

### Notes

- `com.runstarsystems.ecs/package.json` should declare `com.runstarsystems.order` as a normal versioned dependency.
- The consuming Unity project's `Packages/manifest.json` should include Git URLs for both `com.runstarsystems.order` and `com.runstarsystems.ecs` when using GitHub packages directly.
- Installing `com.runstarsystems.order` manually before installing `com.runstarsystems.ecs` should resolve missing dependency errors if the package name and version match.
- GitHub packages should not be treated as fully automatic updates because Unity may pin package revisions in `packages-lock.json`.
- `rootNamespace` does not create an assembly dependency.
- `.asmdef` references must use the referenced assembly's `name` field.
- C# namespaces can span multiple assemblies.
- C# `using` directives are not transitive.
- C# does not support star/wildcard namespace imports.
- `Get<TAttribute>()` returns direct and inherited matched types as long as inherited matches were added into the registry.
- `GetMatches(...)` should be used when an organizer needs the actual metadata object, the source type, or the inherited/direct state.
- Inheritance should resolve before later organizers consume registry metadata.
- Reflection remains direct metadata only.
- The registry is the effective metadata layer.
- The inherit cache is an optimization and organization layer, not the main source of truth.
- Priority subgroup ordering may remain in the project temporarily to avoid dependency pain, but its long-term direction is a separate package or assembly.

## 1.0.1

### Added

- Added reusable Runstar bootstrap pipeline separate from the Unity `ICustomBootstrap` entry point.
- Added plugin-style organizer pipeline for running ECS organization logic before and after Unity default system insertion.
- Added organizer priority support through `RunstarOrganizerPriorityAttribute`.
- Added pre-default organizer range for Runstar setup before Unity inserts remaining systems.
- Added post-default organizer range for logic that must run after Unity default system insertion.
- Added `RunstarTypeRegistry` for collecting and caching attribute/type matches before organizers run.
- Added registry-based type removal so Runstar-owned systems and groups are removed before Unity default insertion.
- Added support for organizer-declared attributes so type scanning can be shared instead of repeated per organizer.
- Added `RunstarBootStrapPipeline` as the shared pipeline entry point for organizer creation, attribute registration, type removal, and organizer execution.
- Added separated offline bootstrap assembly definition.
- Added optional bootstrap compilation control so the offline bootstrap can be disabled when another bootstrap package is present.
- Added package-version detection support for disabling the offline bootstrap when `com.runstarsystems.netcode` is installed.
- Added package-version detection support for disabling the offline bootstrap when `com.custom.bootstrap` is installed.

### Changed

- Moved most bootstrap logic out of `RunstarBootstrap` and into the shared pipeline.
- Changed `RunstarBootstrap` into a thin offline bootstrap wrapper.
- Changed organizer execution to use the shared pipeline instead of being owned directly by the bootstrap class.
- Changed Runstar type ownership so registered Runstar types are removed from Unity's system list before `DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(...)`.
- Changed the runtime assembly layout to separate core ECS logic from the optional offline bootstrap.
- Changed the bootstrap design to prepare for Netcode-specific world setup.
- Changed the type organization model to support shared scanned metadata before per-world registration.
- Changed the pipeline direction so future client, server, thin-client, and custom bootstrap flows can reuse the same core organizer logic.

### Notes

- `RunstarSystems.ECS.Core` should contain the reusable runtime code, organizers, attributes, groups, registry, and pipeline.
- `RunstarSystems.ECS.OfflineBootstrap` should contain only the default offline `ICustomBootstrap`.
- The offline bootstrap assembly should not compile when `com.runstarsystems.netcode` or `com.custom.bootstrap` is present.
- Netcode support is expected to use the same core pipeline but create separate client, server, and thin-client contexts.
- Registry data is intended to be shared globally, while type consumption remains per-world or per-context.
- Attribute order does not affect behavior; attributes are collected before organizer logic runs.
- Priority groups still provide absolute ordering inside the Runstar fixed, update, and late pipelines.
- Unity still handles remaining unconsumed systems normally through default ECS insertion.

## 1.0.0

### Added

- Added priority group support for ordered ECS groups.
- Added update priority group ordering.
- Added fixed priority group ordering.
- Added sample showing how systems connect to priority groups.
- Added authored ECS data sample.
- Added example flow for editor settings, author components, bakers, and runtime ECS data.
- Added sample debug system for checking baked ECS component data.
- Added package structure for Unity Package Manager.
- Added runtime assembly definition for package code.
- Added optional samples through `Samples~`.

### Notes

- Samples are optional and can be imported from Unity Package Manager.
- Runtime code is kept separate from sample code.
- Editor data is converted into ECS data through bakers.
- Priority groups work like normal ECS groups, but with explicit order values.
