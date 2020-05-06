using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaviours
{
	public class FoodSpawner : SpectralMonoBehavior
	{
		private const float CHECK_SPAWN_COOLDOWN = 2;
		private const float PER_SPAWN_DELAY_MAX = 2;

		private static PoolableObject<FoodObject>[] FoodObjectPools;

		private float checkSpawnCooldown;
		private bool spawning;
		private void Start()
		{
			SetupFoodObjectPools();
		}
		private void SetupFoodObjectPools()
		{
			FoodObjectPools = new PoolableObject<FoodObject>[GameManager.CurrentGameSettings.FoodObjectVariants.Length];
			for (int i = 0; i <  FoodObjectPools.Length; i++)
			{
				FoodObjectPools[i] = new PoolableObject<FoodObject>(0, true, GameManager.CurrentGameSettings.FoodObjectVariants[i], Storage.FoodObjectStorage);
			}
		}
		private void Update()
		{
			if(checkSpawnCooldown > 0)
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

			int totalRequired = GameManager.CurrentLevelSettings.FoodInLevel.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultFoodInLevelCount);
			int dif = totalRequired - FoodObject.AllFoodObjects.Count;
			if (dif > 0)
			{
				StartCoroutine(SpawnFood(dif));
			}
		}
		private IEnumerator SpawnFood(int count = 1)
		{
			spawning = true;
			float levelWidht = GameManager.CurrentLevelSettings.LevelWidht.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelWidht);
			float levelHeight = GameManager.CurrentLevelSettings.LevelHeight.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelHeight);

			for (int i = 0; i < count; i++)
			{
				yield return new WaitForSeconds(PER_SPAWN_DELAY_MAX * Random.value);

				Vector3 spawnPos = new Vector3(levelWidht * (Random.value -0.5f), 0, levelHeight * (Random.value -0.5f));
				FoodObject spawnedObject = FoodObjectPools[Random.Range(0, FoodObjectPools.Length)].GetPoolObject();
				spawnedObject.transform.position = spawnPos;
			}
			spawning = false;
		}
	}
}