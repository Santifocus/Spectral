using System.Collections.Generic;
using Spectral.Runtime.Behaviours.Entities;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class FoodObject : SpectralMonoBehavior, IPoolableObject<FoodObject>
	{
		private const float CHECK_FOR_DE_SPAWN_COOLDOWN = 3;
		public static readonly List<FoodObject> AllFoodObjects = new List<FoodObject>();
		public PoolableObject<FoodObject> SelfPool { get; set; }
		[SerializeField] private float expandTime = 1.5f;

		private bool isEdible;

		private int expandDir;
		private float expandTimer;
		private float checkForDeSpawnCooldown;

		private void Update()
		{
			if (checkForDeSpawnCooldown > 0)
			{
				checkForDeSpawnCooldown -= Time.deltaTime;
			}
			else
			{
				CheckForDeSpawn();
			}

			if (expandDir != 0)
			{
				expandTimer += Time.deltaTime * expandDir;
				transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, expandTimer / expandTime);
				if ((expandTimer >= 1) && (expandDir == 1))
				{
					expandTimer = 1;
					expandDir = 0;
				}
				else if ((expandTimer <= 0) && (expandDir == -1))
				{
					expandTimer = 0;
					expandDir = 0;
					ReturnToPool();
				}
			}
		}

		private void CheckForDeSpawn()
		{
			if (ShouldDeSpawn())
			{
				expandDir = -1;
			}
			else
			{
				checkForDeSpawnCooldown = CHECK_FOR_DE_SPAWN_COOLDOWN;
			}
		}

		protected virtual bool ShouldDeSpawn()
		{
			return transform.position.XYZtoXZ().RequiresLevelBoundsClamping();
		}

		private void OnEnable()
		{
			transform.localScale = Vector3.zero;
			isEdible = true;
			expandDir = 1;
			AllFoodObjects.Add(this);
		}

		private void OnDisable()
		{
			AllFoodObjects.Remove(this);
		}

		public void ReturnToPool()
		{
			isEdible = false;
			if (SelfPool != null)
			{
				SelfPool.ReturnPoolObject(this);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public virtual void Eat(EntityMover from)
		{
			isEdible = false;
			expandDir = -1;
			from.OnEat(this);
		}

		public static FoodObject GetNearestFoodObject(Vector2 point, float? maxRange = null)
		{
			float shortestDist = (maxRange * maxRange) ?? Mathf.Infinity;
			int targetIndex = -1;
			for (int i = 0; i < AllFoodObjects.Count; i++)
			{
				if (!AllFoodObjects[i].isActiveAndEnabled || !AllFoodObjects[i].isEdible)
				{
					continue;
				}

				float dist = (AllFoodObjects[i].transform.position.XYZtoXZ() - point).sqrMagnitude;
				if (dist < shortestDist)
				{
					shortestDist = dist;
					targetIndex = i;
				}
			}

			return targetIndex >= 0 ? AllFoodObjects[targetIndex] : null;
		}
	}
}