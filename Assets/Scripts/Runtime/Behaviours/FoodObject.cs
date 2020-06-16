using System;
using Spectral.Runtime.Interfaces;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class FoodObject : LevelPlaneBehavior, IPoolableObject<FoodObject>
	{
		private const float CHECK_FOR_DE_SPAWN_COOLDOWN = 3;
		public ObjectPool<FoodObject> SelfPool { get; set; }
		[SerializeField] private float expandTime = 1.5f;

		public bool IsEdible { get; private set; }

		private FoodSpawner affiliatedSpawner;
		private bool initiated;

		private int expandDir;
		private float expandTimer;
		private float checkForDeSpawnCooldown;

		private void Start()
		{
			if (!initiated)
			{
				ForceInitiation();
			}
		}

		public void Initialise(FoodSpawner affiliatedSpawner)
		{
			initiated = true;
			this.affiliatedSpawner = affiliatedSpawner;
		}

		public void Setup(Vector3 spawnPos)
		{
			transform.localScale = Vector3.zero;
			transform.localPosition = spawnPos;
			IsEdible = true;
			expandDir = 1;
			if (affiliatedSpawner)
			{
				affiliatedSpawner.ActiveFoodObjects.Add(this);
			}
		}

		private void TearDown()
		{
			IsEdible = false;
			if (affiliatedSpawner)
			{
				affiliatedSpawner.ActiveFoodObjects.Remove(this);
			}
		}

		private void ForceInitiation()
		{
			FoodSpawner acquiredAffiliatedSpawner = AffiliatedLevelPlane.AffiliatedFoodSpawner;
			if (acquiredAffiliatedSpawner == null)
			{
				gameObject.SetActive(false);
#if SPECTRAL_DEBUG
				throw new SystemException($"Food object was created without a {nameof(affiliatedSpawner)}, and could not find one to which it was attached to.");
#endif
			}
			else
			{
				Initialise(acquiredAffiliatedSpawner);
				Setup(transform.localPosition);
			}
		}

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

			ExpandControl();
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
			return transform.position.XYZtoXZ().RequiresLevelBoundsClamping(AffiliatedLevelPlane.PlaneSettings);
		}

		private void ExpandControl()
		{
			if (expandDir == 0)
			{
				return;
			}

			expandTimer += Time.deltaTime * expandDir;
			float expandState = expandTimer / expandTime;
			transform.localScale = Vector3.one * expandState;
			if ((expandState >= 1) && (expandDir == 1))
			{
				FinishExpansion(true);
			}
			else if ((expandState <= 0) && (expandDir == -1))
			{
				FinishExpansion(false);
			}
		}

		public void FinishExpansion(bool expanded)
		{
			expandDir = 0;
			expandTimer = expanded ? 1 : 0;
			if (!expanded)
			{
				ReturnToPool();
			}

			transform.localScale = Vector3.one * expandTimer;
		}

		public void ReturnToPool()
		{
			TearDown();
			if (SelfPool != null)
			{
				SelfPool.ReturnPoolObject(this);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public virtual void Eat()
		{
			IsEdible = false;
			expandDir = -1;
		}
	}
}