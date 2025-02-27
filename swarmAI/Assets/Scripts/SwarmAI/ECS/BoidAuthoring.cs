using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BoidAuthoring : MonoBehaviour
{
   public float2 Offset;
	public class BoidBaker : Baker<BoidAuthoring>
	{
		public override void Bake(BoidAuthoring authoring) {
			var entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new BoidTag {
				Offset = authoring.Offset
			});
		}
	}
}
