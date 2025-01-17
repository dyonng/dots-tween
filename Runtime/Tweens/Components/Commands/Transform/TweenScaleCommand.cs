﻿using Unity.Burst;
using Unity.Entities;

namespace DotsTween.Tweens
{
    [BurstCompile]
    public struct TweenScaleCommand : IComponentData, ITweenParams, ITweenInfo<float>
    {
        public TweenParams TweenParams;
        public float Start;
        public float End;

        public TweenScaleCommand(in Entity entity, in float start, in float end, in float duration, TweenParams tweenParams = default)
        {
            tweenParams.Duration = duration;
            tweenParams.Id = tweenParams.GenerateId(entity.GetHashCode(), start.GetHashCode(), end.GetHashCode(), TypeManager.GetTypeIndex<TweenScale>().Value);
            TweenParams = tweenParams;
            Start = start;
            End = end;
        }

        [BurstCompile]
        public void SetTweenInfo(in float start, in float end)
        {
            Start = start;
            End = end;
        }

        [BurstCompile]
        public void SetTweenParams(in TweenParams tweenParams)
        {
            TweenParams = tweenParams;
        }

        [BurstCompile]
        public TweenParams GetTweenParams()
        {
            return TweenParams;
        }

        [BurstCompile]
        public float GetTweenStart()
        {
            return Start;
        }

        [BurstCompile]
        public float GetTweenEnd()
        {
            return End;
        }

        [BurstCompile] public void Cleanup() { }
    }
}