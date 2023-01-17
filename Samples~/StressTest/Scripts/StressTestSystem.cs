﻿using System;
using DotsTween.Tweens;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace DotsTween.Samples.StressTest
{
    public partial class StressTestSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
#if !ENTITYTWEEN_HYBRID_RENDERER
            Debug.LogWarning("Please install Hybrid Renderer (com.unity.rendering.hybrid) to enable rendering.");
#endif
        }

        protected override void OnUpdate()
        {
            EndSimulationEntityCommandBufferSystem endSimECBSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer.ParallelWriter parallelWriter = endSimECBSystem.CreateCommandBuffer().AsParallelWriter();
            Random random = new Random((uint) Environment.TickCount);

            Entities.ForEach((Entity entity, int entityInQueryIndex, in StressTestCommand cmd) =>
            {
                for (int i = 0; i < cmd.Count; i++)
                {
                    Entity obj = parallelWriter.Instantiate(entityInQueryIndex, cmd.Prefab);

                    float3 moveStart = random.NextFloat3Direction() * cmd.StartMoveRadius;
                    float3 moveEnd = random.NextFloat3Direction() * cmd.EndMoveRadius;
                    Tween.Move(parallelWriter, entityInQueryIndex, obj, moveStart, moveEnd, cmd.MoveDuration, cmd.MoveEaseType, cmd.MoveIsPingPong, cmd.MoveLoopCount);

                    quaternion rotateEnd = quaternion.AxisAngle(random.NextFloat3Direction(), random.NextFloat(cmd.MinRotateDegree, cmd.MaxRotateDegree));
                    Tween.Rotate(parallelWriter, entityInQueryIndex, obj, quaternion.identity, rotateEnd, cmd.RotateDuration, cmd.RotateEaseType, cmd.RotateIsPingPong, cmd.RotateLoopCount);
                    
                    float scaleStart = random.NextFloat(cmd.MinStartScale, cmd.MaxStartScale);
                    float scaleEnd = random.NextFloat(cmd.MinEndScale, cmd.MaxEndScale);
                    Tween.Scale(parallelWriter, entityInQueryIndex, obj, scaleStart, scaleEnd, cmd.ScaleDuration, cmd.ScaleEaseType, cmd.ScaleIsPingPong, cmd.ScaleLoopCount);
                }

                parallelWriter.RemoveComponent<StressTestCommand>(entityInQueryIndex, entity);
            }).ScheduleParallel();

            endSimECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
