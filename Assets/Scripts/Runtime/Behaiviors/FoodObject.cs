using System;
using Spectral.Runtime.Interfaces;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class FoodObject : SpectralMonoBehavior, IPoolableObject<FoodObject>
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

		public void Initiate(FoodSpawner affiliatedSpawner)
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
			affiliatedSpawner.ActiveFoodObjects.Add(this);
		}

		private void TearDown()
		{
			IsEdible = false;
			affiliatedSpawner.ActiveFoodObjects.Remove(this);
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
				Initiate(acquiredAffiliatedSpawner);
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

			expandTimer += Time.deltaTime      * expandDir;
			transform.localScale = Vector3.one * (expandTimer / expandTime);
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