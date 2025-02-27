using Unity.Entities;
using UnityEngine;

public class BoidSpawnAuthoring : MonoBehaviour
{
	public GameObject Prefab;
	public int SpawnCount;

	public class BoidSpawnDataBaker : Baker<BoidSpawnAuthoring>
	{
		public override void Bake(BoidSpawnAuthoring authoring) {
			var entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new BoidSpawnData {
				Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
				SpawnCount = authoring.SpawnCount
			});
		}
	}
}
