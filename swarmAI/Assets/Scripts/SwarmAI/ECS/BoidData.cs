using Unity.Entities;
using Unity.Mathematics;

public struct BoidTargetTag : IComponentData { }

public struct BoidTag : IComponentData { 
	public float2 Offset;
	public float2 Rule1;
	public float2 Rule2;
	public float2 Rule3;
	public float2 Rule4;

}

public struct BoidSpawnData : IComponentData
{
	public Entity Prefab;
	public int SpawnCount;
}

