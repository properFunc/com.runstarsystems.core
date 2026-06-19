# Inheritance Sample

This sample shows how Runstar attributes can be inherited from a parent group.

## What this sample shows

- How to make an inheritable attribute.
- How to mark a parent group with inheritable metadata.
- How a child system inherits that metadata through `InheritFromGroup`.
- How an organizer reads direct and inherited metadata from the registry.

## Inheritable attribute

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class InheritableDummyAttribute : RunstarInheritableAttribute
{
    public InheritableDummyAttribute(
            InheritMode inheritMode = InheritMode.DirectOnly)
            : base(inheritMode)
    {
    }
}
```

`RunstarInheritableAttribute` is the parent class for metadata that can be inherited.

## Parent group

```csharp
[InheritableDummy(InheritMode.Inheritable)]
[ECSLateGroupOrder(500)]
public partial class InheritExampleGroup : ComponentSystemGroup
{
}
```

`InheritMode.Inheritable` allows this metadata to be passed to children.

## Child system

```csharp
[InheritFromGroup(typeof(InheritExampleGroup))]
public partial class InheritExampleSystem : SystemBase
{
}
```

`InheritFromGroup` places the system in the parent group and lets it inherit the parent's inheritable metadata.

## Basic organizer

```csharp
[OrganizerPriority(500)]
public sealed class InheritSampleOrganizer : IRunstarOrganizer
{
    public IReadOnlyList<Type> GetAttributeTypes()
    {
        return new Type[] { typeof(InheritableDummyAttribute) };
    }

    public void Register(RunstarOrganizerContext context)
    {
        var matches = context.TypeRegistry.GetMatches
                <InheritableDummyAttribute,
                 InheritableDummyAttribute>();

        // matches contains direct parent metadata and inherited child metadata.
    }
}
```

## Notes

- Use `Get<TAttribute>()` when only the matched types are needed.
- Use `GetMatches<TAttribute, TMetadata>()` when metadata details are needed.
- Reflection only sees direct attributes.
- The registry is the effective metadata source after inheritance resolves.
