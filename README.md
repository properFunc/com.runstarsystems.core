# Runstar ECS

Runstar ECS is a small Unity ECS package for organizing game system order, metadata, and bootstrap setup.

It was made because relative ECS ordering and scattered attributes can become hard to follow as a project grows. Runstar keeps system organization more direct by using explicit group order, shared metadata scanning, and organizer-based setup.

```text
100 -> Input
200 -> Network
300 -> Rollback
400 -> Simulation
500 -> Output
```

Systems still connect to groups the same way they connect to normal Unity ECS groups.

## Included

- Reusable bootstrap pipeline
- Organizer priority system
- Type registry for attribute metadata
- Metadata inheritance support
- Ordered fixed, update, and late groups
- Group organizer utilities
- Authored ECS data sample
- Priority group sample
- Inheritance sample

## Philosophy

I find it important to make this at some point. Having exact reasons
to do something can help make alignment easier. So will do at some point.

## Test and Documentation

On its way!!!

## Goal

Runstar ECS is meant to grow as more reusable game systems are built.

The goal is to keep ECS projects easier to read, debug, and expand without hiding how Unity ECS works.
