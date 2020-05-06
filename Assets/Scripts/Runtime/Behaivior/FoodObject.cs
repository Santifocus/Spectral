using Spectral.Behaviours.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaviours
{
	public class FoodObject : SpectralMonoBehavior, IPoolableObject<FoodObject>
	{
		private static readonly List<FoodObject> AllFoodObjects = new List<FoodObject>();
		public PoolableObject<FoodObject> SelfPool { get; set; }

		private void Start()
		{
			AllFoodObjects.Add(this);
		}
		private void OnDestroy()
		{
			AllFoodObjects.Remove(this);
		}
		public void ReturnToPool()
		{
			SelfPool.ReturnPoolObject(this);
		}
		public virtual void Eat(EntityMover from)
		{
			from.OnEat(this);
			if (SelfPool != null)
			{
				ReturnToPool();
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public static FoodObject GetNearestFoodObject(Vector2 point, float maxRange = Mathf.Infinity)
		{
			float shortestDist = Mathf.Infinity;
			int targetIndex = -1;
			for (int i = 0; i < AllFoodObjects.Count; i++)
			{
				if (!AllFoodObjects[i].isActiveAndEnabled)
				{
					continue;
				}

				float dist = (AllFoodObjects[i].transform.position.XYZtoXZ() - point).sqrMagnitude;
				if ((dist < (maxRange * maxRange)) && (dist < shortestDist))
				{
					shortestDist = dist;
					targetIndex = i;
				}
			}

			return (targetIndex >= 0) ? AllFoodObjects[targetIndex] : null;
		}
	}
}