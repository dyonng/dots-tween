﻿#if DOTS_TWEEN_URP
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace DotsTween.Tweens
{
    [BurstCompile]
    [UpdateInGroup(typeof(TweenApplySystemGroup))]
    internal partial class TweenURPSmoothnessSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate<TweenURPSmoothness>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach((ref URPMaterialPropertySmoothness smoothness, in DynamicBuffer<TweenState> tweenBuffer, in TweenURPSmoothness tweenInfo) =>
            {
                foreach (var tween in tweenBuffer)
                {
                    if (tween.Id != tweenInfo.Id || tween.IsPaused) continue;
                    smoothness.Value = math.lerp(tweenInfo.Start, tweenInfo.End, tween.EasePercentage);
                }
            }).ScheduleParallel();
        }
    }
}
#endif