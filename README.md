# Entity Tween

[//]: # (![GitHub release &#40;latest SemVer&#41;]&#40;https://img.shields.io/github/v/release/nagachiang/entity-tween?sort=semver&#41; ![Unity]&#40;https://github.com/NagaChiang/entity-tween/workflows/Unity/badge.svg&#41;)

Entity compatible tween library for Unity ECS/DOTS

## Table of Contents

- [Demonstration](#demonstration)
- [Features](#features)
- [Dependencies](#dependencies)
- [Installation](#installation)
- [Examples](#examples)
  - [Move the entity](#move-the-entity)
  - [Stop the entity](#stop-the-entity)
  - [Loop infinitely](#loop-infinitely)
  - [Check if the entity is tweening](#check-if-the-entity-is-tweening)
- [Workflow](#workflow)
  - [Command](#command)
  - [Generation](#generation)
  - [Easing](#easing)
  - [Applying](#applying)
  - [Checking State](#checking-state)
  - [Destroying](#destroying)
- [Donation](#donation)

## Demonstration

![](https://i.imgur.com/3GM0RCE.gif)

[Link to the larger gif](https://i.imgur.com/3oZnviK.gif)

### Configuration

- 70000 tweening entities
    - `Translation`
    - `Rotation`
    - `NonUniformScale`
- Burst
    - Leak Detection: Off
    - Safety Checks: Off
    - Synchronous Compilation: On
    - Jobs Debugger: Off
- GPU instancing

### Hardware

- Intel i7-8700 @ 3.2GHz
- NVIDIA GeForce GTX 1660 Ti

## Features

- Tween support
    - `Translation.Value`
    - `Rotation.Value`
    - `NonUniformScale.Value`
    - `SpriteRenderer.Color` in Tiny
- Pause, resume and stop tweens on an entity
- Multiple types of active tweens on the same entity at the same time
- Ping-pong
- Loop
- Start delay
- Ease library (inspired by [Squirrel Eiserloh's talk on GDC 2015](https://www.youtube.com/watch?v=mr5xkf6zSzk))
    - Smooth start
    - Smooth stop
    - Smooth step
    - Crossfade

## Dependencies

- "com.unity.collections": "2.1.0-pre.6",
- "com.unity.entities": "1.0.0-pre.15",
- "com.unity.burst": "1.8.2",
- "com.unity.mathematics": "1.2.6"

## Installation

Entity Tween is a Unity package. You can [install it from the git URL](https://docs.unity3d.com/2020.1/Documentation/Manual/upm-ui-giturl.html) in Unity package manager.

Or, you can edit `Packages/manifest.json` manually, adding git URL as a dependency:

```json
"dependencies": {
    "com.dyonng.dotstween": "https://github.com/dyonng/dots-tween.git"
}
```

For more information, please visit [Unity documentation](https://docs.unity3d.com/2020.1/Documentation/Manual/upm-git.html).

## Examples

The main entry point of the library is the `Tween` class. All functionality have overloads to support `EntityManager`, `EntityCommandBuffer` and `EntityCommandBuffer.ParallelWriter`.

### Move the entity

```cs
float3 start = new float3(0.0f, 0.0f, 0.0f);
float3 end = new float3(1.0f, 1.0f, 1.0f);
float duration = 5.0f;
bool isPingPong = false;
int loopCount = 1;

Tween.Move(entityManager, entity, start, end, duration, EaseDesc.SmoothStep, isPingPong, loopCount);
Tween.Move(commandBuffer, entity, start, end, duration, EaseDesc.SmoothStep, isPingPong, loopCount);
Tween.Move(parallelWriter, sortKey, entity, start, end, duration, EaseDesc.SmoothStep, isPingPong, loopCount);
```

### Stop the entity

```cs
Tween.Stop(entityManager, entity);
```

### Loop infinitely

When `loopCount` is 0, it means loop the tween infinitely. It's recommended to use `Tween.Infinite` in case it changes in the future.

```cs
Tween.Move(entityManager, entity, start, end, duration, EaseDesc.SmoothStep, isPingPong, Tween.Infinite);
```

### Check if the entity is tweening

Any entity with component `TweenState` is tweening; that is, to know if the entity is tweening, check if the entity has any `TweenState` component in any way.

```cs
if (EntityManager.HasComponent<TweenState>(entity))
{
    Debug.Log("It's tweening!");
}
```

## Workflow

### Command

When starting a tween by calling `Tween`'s functions (e.g. `Tween.Move()`), it creates a tween command component of its kind (e.g. `TweenTranslationCommand`) containing the tweening data on the target entity.

If starting multiple tweens with the same type consequently, the command component will be overridden by the last one, which means only the last tween will be successfully triggered.

### Generation

`TweenGenerateSystem` is an abstract generic system, which will take the commands of its kind, then append a `TweenState` with an unique tween ID to the `DynamicBuffer` of the target entity. Also, it creates another component with the same tween ID, storing the start and end information of the tween.

Making `TweenState` an `IBufferElementData` allows multiple active tweens on the same entity at the same time, while the other part of the tween data, e.g. `TweenTranslation`, is an `IComponentData`, which ensures that there must be only one tween of the certain type.

For example, `TweenTranslationGenerateSystem`, which inherits `TweenGenerateSystem`, will take `TweenTranslationCommand` then create `TweenState` and `TweenTranslation` on the target entity.

### Easing

`TweenEaseSystem` iterates all `TweenState` components and update the elapsed time of the tween, then calculate the progress by `Ease.CalculatePercentage()` for later use.

### Applying

For every type of tweens, there is a system responsible for applying the value to the target component.

For example, `TweenTranslationSystem` takes `TweenState` and `TweenTranslation` and interpolate the value depending on the eased percentage, then apply it to `Translation`.

### Checking State

`TweenStateSystem` iterates all `TweenState` components checking if they're about to be looped, ping-ponged or destroyed. The tween will loop infinitely when its `TweenState.LoopCount == 0`; however, if it just ticked to 0 from 1, the system will mark it as pending destroy with `TweenDestroyCommand`;

### Destroying

`TweenDestroySystem` is also an abstract generic system for each type of tween to implement, which destroys `TweenState` marked by `TweenStateSystem` earlier and its corresponding tween data component.

For example, `TweenTranslationDestroySystem` will be responsible for destroying `TweenState` and `TweenTranslation`.

## Donation

### NagaChiang's Donation Link (Original Author):
[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/C0C12EHR2)