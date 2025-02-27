using JetBrains.Annotations;
using System.Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct BoidMoveSystem : ISystem, ISystemStartStop
{
	private EntityQuery _boidQuery;

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		//state.RequireForUpdate<BoidTargetTag>();
		_boidQuery = SystemAPI.QueryBuilder().WithAll<BoidTag>().Build();

	}

	public void OnStartRunning(ref SystemState state) {
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		var boidEntityArray = _boidQuery.ToEntityArray(state.WorldUpdateAllocator);
		var pos = new NativeArray<float2>(boidEntityArray.Length, Allocator.TempJob);
		var diffs = new NativeArray<float2>(boidEntityArray.Length, Allocator.TempJob);
		var offsets = new NativeArray<float2>(boidEntityArray.Length, Allocator.TempJob);

		float2 averageCenter = float2.zero;
		float2 averageSpeed = float2.zero;
		//float2 ruleTwo;
		int size = boidEntityArray.Length;
		for (var i = 0; i < size; i++) {
			pos[i] = SystemAPI.GetComponent<LocalTransform>(boidEntityArray[i]).Position.xy;
			averageCenter += pos[i];
			offsets[i] = SystemAPI.GetComponent<BoidTag>(boidEntityArray[i]).Offset;
			averageSpeed += offsets[i];
		}
		new BoidUpdatePos {
			AverageCenter = averageCenter,
			AverageSpeed = averageSpeed,
			DeltaTime = SystemAPI.Time.DeltaTime,
			Pos = pos,
			Size = size,
			Offsets = offsets,
			CenterInfluence = 0.05f,
			SeparationInfluence = 2f,
			MatchInfluence = 0.1f,
			BoundInfluence = 1f,
			TargetInfluence = 1f

		}.ScheduleParallel();
		//for (var i = 0; i < size; i++) {
		//	float2 diff = float2.zero;
		//	//rule1
		//	diff += (averageCenter - pos[i]) / (size - 1) - pos[i];
		//	//rule2
		//	ruleTwo = float2.zero;
		//	for (var j = 0; j < size; j++) {
		//		if (i != j) {
		//			if (math.distancesq(pos[i], pos[j]) < 12f) {
		//				ruleTwo -= pos[i] - pos[j];
		//			}
		//		}
		//	}
		//	diff += ruleTwo;
		//	//rule3
		//	diff += (averageSpeed - SystemAPI.GetComponent<BoidTag>(boidEntityArray[i]).Offset) / (size - 1) - SystemAPI.GetComponent<BoidTag>(boidEntityArray[i]).Offset;
		//	//rule 4
		//	if (pos[i].x < -50f) {
		//		diff.x += 1;
		//	} else if (pos[i].x > 50f) {
		//		diff.x -= -1;
		//	}
		//	if (pos[i].y < -50f) {
		//		diff.y += 1;
		//	} else if (pos[i].y > 50f) {
		//		diff.y -= -1;
		//	}
		//	diffs[i] = diff;
		//}

		//new BoidUpdatePos { DeltaTime = SystemAPI.Time.DeltaTime, Diffs = diffs }.ScheduleParallel();

	}

	public void OnStopRunning(ref SystemState state) {
	}

	public void OnDestroy(ref SystemState state) {
	}
}

[BurstCompile]
public partial struct BoidUpdatePos : IJobEntity
{
	[ReadOnly]
	public float DeltaTime;
	[ReadOnly]
	public int Size;
	[ReadOnly]
	public float2 AverageSpeed;
	[ReadOnly]
	public float2 AverageCenter;
	[ReadOnly]
	public NativeArray<float2> Pos;
	[ReadOnly]
	public NativeArray<float2> Offsets;
	[ReadOnly]
	public float CenterInfluence, SeparationInfluence, MatchInfluence, BoundInfluence, TargetInfluence;

	[BurstCompile]
	private readonly void Execute(ref LocalTransform transform, ref BoidTag boidData) {
		float2 diff = float2.zero;
		float2 pos = new(transform.Position.x, transform.Position.y);
		//rule1
		diff += ((AverageCenter - pos) / (Size - 1) - pos) * CenterInfluence;
		//boidData.Rule1 = diff;
		//rule2
		float2 ruleTwo = float2.zero;
		for (var j = 0; j < Size; j++) {
			if (math.distancesq(pos, pos[j]) > 0f && math.distancesq(pos, pos[j]) < 12f) {
				ruleTwo -= pos - pos[j];
			}
		}
		diff += ruleTwo * SeparationInfluence;
		//boidData.Rule2 = ruleTwo;
		//rule3
		diff += ((AverageSpeed - boidData.Offset / (Size - 1)) - boidData.Offset) * MatchInfluence;
		//boidData.Rule3 = diff;
		//rule 4
		//if (pos.x < -50f) {
		//	diff.x += 1;
		//} else if (pos.x > 50f) {
		//	diff.x -= 1;
		//}
		//if (pos.y < -50f) {
		//	diff.y += 1;
		//} else if (pos.y > 50f) {
		//	diff.y -= 1;
		//}
		//rule4 elastic
		boidData.Rule3 = pos/10f;
		diff -= pos/10f;
		//rule5 target as 0,0
		diff += (-pos / 100f) * TargetInfluence;

		boidData.Rule1 = diff;
		diff *= DeltaTime;
		if (math.length(diff) > 5f) {
			diff = (diff / math.length(diff)) * 5f;
		}
		boidData.Rule2 = diff;
		float3 newPos = transform.Position;
		newPos.x += diff.x;
		newPos.y += diff.y;
		boidData.Offset += diff;
		transform.Position = newPos;
	}
}