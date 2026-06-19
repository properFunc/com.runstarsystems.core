# Authored ECS Data Sample

This sample shows how to connect editor data to ECS data.

The author component holds editable Unity Inspector data. The baker converts that data into ECS components during baking.

## What this sample shows

- How to make ECS component data.
- How to make editor settings data.
- How to add settings to a GameObject.
- How to bake editor data into ECS data.
- How to check the baked data in a system.

## Basic flow

```text
ECS Data
    Data used by systems at runtime.

Editor Settings
    Serializable data shown in the Unity Inspector.

Author
    MonoBehaviour added to a GameObject.
    Holds the editor settings.

Baker
    Reads the author data during baking.
    Converts editor settings into ECS data.

Debug System
    Reads the baked ECS data.
    Prints the values to confirm the bake worked.
```

## Data path

The game object only need the author script
It is the only one that can be added because it
is the only one with monobehavior
```text
GameObject
    -> Author
```

## Notes

The editor settings are separate from the ECS data so the Inspector can use Unity friendly types and validation.

The baker is the bridge between the GameObject authoring world and the ECS runtime world.
