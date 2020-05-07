using Spectral.Behaviours.Entities;
using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaviours
{
	public class FoodObject : SpectralMonoBehavior, IPoolableObject<FoodObject>
	{
		private const float CHECK_FOR_DESPAWN_COOLDOWN = 3;
		public static readonly List<FoodObject> AllFoodObjects = new List<FoodObject>();
		public PoolableObject<FoodObject> SelfPool { get; set; }
		[SerializeField] private float expandTime = 1.5f;

		private bool isEdible;

		private int expandDir;
		private float expandTimer;
		private float checkForDespawnCooldown;
		private void Update()
		{
			if(checkForDespawnCooldown > 0)
			{
				checkForDespawnCooldown -= Time.deltaTime;
			}
			else
			{
				CheckForDespawn();
			}
			if(expandDir != 0)
			{
				expandTimer += Time.deltaTime * expandDir;
				transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, expandTimer / expandTime);

				if((expandTimer >= 1) && (expandDir == 1))
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
		private void CheckForDespawn()
		{
			if (ShouldDespawn())
			{
				expandDir = -1;
			}
			else
			{
				checkForDespawnCooldown = CHECK_FOR_DESPAWN_COOLDOWN;
			}
		}
		protected virtual bool ShouldDespawn()
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

			return (targetIndex >= 0) ? AllFoodObjects[targetIndex] : null;
		}
	}
}