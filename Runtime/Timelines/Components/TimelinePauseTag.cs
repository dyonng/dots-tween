﻿using Unity.Burst;
using Unity.Entities;

namespace DotsTween.Timelines
{
    [BurstCompile]
    public struct TimelinePausedTag : IComponentData
    {
    }
}