using System.Collections;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class FoodSpawner : SpectralMonoBehavior
	{
		private const float CHECK_SPAWN_COOLDOWN = 2;
		private const float PER_SPAWN_DELAY_MAX = 2;

		private static PoolableObject<FoodObject>[] FoodObjectPools;

		private float checkSpawnCooldown;

		private void Start()
		{
			SetupFoodObjectPools();
		}

		private void SetupFoodObjectPools()
		{
			FoodObjectPools = new PoolableObject<FoodObject>[GameSettings.Current.FoodObjectVariants.Length];
			for (int i = 0; i < FoodObjectPools.Length; i++)
			{
				FoodObjectPools[i] = new PoolableObject<FoodObject>(0, true, GameSettings.Current.FoodObjectVariants[i], Storage.FoodObjectStorage);
			}
		}

		private void Update()
		{
			if (checkSpawnCooldown > 0)
			{
				checkSpawnCooldown -= Time.deltaTime;
			}
			else
			{
				CheckForSpawn();
			}
		}

		private void CheckForSpawn()
		{
			checkSpawnCooldown = CHECK_SPAWN_COOLDOWN;
			int totalRequired = LevelSettings.Current.FoodInLevel;
			int dif = totalRequired - FoodObject.AllFoodObjects.Count;
			if (dif > 0)
			{
				StartCoroutine(SpawnFood(dif));
			}
		}

		private IEnumerator SpawnFood(int count = 1)
		{
			float levelWidth = LevelSettings.Current.LevelWidth;
			float levelHeight = LevelSettings.Current.LevelHeight;
			for (int i = 0; i < count; i++)
			{
				yield return new WaitForSeconds(PER_SPAWN_DELAY_MAX * Random.value);
				Vector3 spawnPos = new Vector3(levelWidth * (Random.value - 0.5f), 0, levelHeight * (Random.value - 0.5f));
				FoodObject spawnedObject = FoodObjectPools[Random.Range(0, FoodObjectPools.Length)].GetPoolObject();
				spawnedObject.transform.position = spawnPos;
			}
		}
	}
}