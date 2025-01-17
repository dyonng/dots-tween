﻿#if DOTS_TWEEN_URP
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace DotsTween.Tweens
{
    [BurstCompile]
    [UpdateInGroup(typeof(TweenApplySystemGroup))]
    internal partial class TweenURPMetallicSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate<TweenURPMetallic>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach((ref URPMaterialPropertyMetallic sheen, in DynamicBuffer<TweenState> tweenBuffer, in TweenURPMetallic tweenInfo) =>
            {
                foreach (var tween in tweenBuffer)
                {
                    if (tween.Id != tweenInfo.Id || tween.IsPaused) continue;
                    sheen.Value = math.lerp(tweenInfo.Start, tweenInfo.End, tween.EasePercentage);
                }
            }).ScheduleParallel();
        }
    }
}
#endif