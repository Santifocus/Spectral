﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class FoodSpawner : MonoBehaviour
	{
		private const float CHECK_SPAWN_COOLDOWN = 2;
		private const float PER_SPAWN_DELAY_MAX = 2;

		public LevelPlane TargetPlane { get; private set; }
		public readonly List<FoodObject> ActiveFoodObjects = new List<FoodObject>();

		private ObjectPool<FoodObject>[] foodObjectPools;
		private float checkSpawnCooldown;

		public void Initialise(LevelPlane targetPlane)
		{
			TargetPlane = targetPlane;
			SetupFoodObjectPools();
		}

		private void SetupFoodObjectPools()
		{
			foodObjectPools = new ObjectPool<FoodObject>[TargetPlane.PlaneSettings.FoodObjectVariants.Length];
			for (int i = 0; i < foodObjectPools.Length; i++)
			{
				FoodObject targetVariant = TargetPlane.PlaneSettings.FoodObjectVariants[i];
				foodObjectPools[i] = new ObjectPool<FoodObject>(0, true, () => CreateNewFoodObject(targetVariant), TargetPlane.TargetStorage.FoodObjectStorage);
			}
		}

		private FoodObject CreateNewFoodObject(FoodObject variant)
		{
			FoodObject newFoodObject = Instantiate(variant);
			newFoodObject.Initialise(this);

			return newFoodObject;
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
			int totalRequired = TargetPlane.PlaneSettings.FoodInLevel;
			int dif = totalRequired - ActiveFoodObjects.Count;
			if (dif > 0)
			{
				StartCoroutine(SpawnFood(dif));
			}
		}

		private IEnumerator SpawnFood(int count = 1)
		{
			float levelWidth = TargetPlane.PlaneSettings.LevelWidth             -
								GameSettings.Current.LevelBorderForceFieldWidth -
								GameSettings.Current.LevelBorderSpawnBlockWidth;

			float levelHeight = TargetPlane.PlaneSettings.LevelHeight           -
								GameSettings.Current.LevelBorderForceFieldWidth -
								GameSettings.Current.LevelBorderSpawnBlockWidth;

			for (int i = 0; i < count; i++)
			{
				yield return new WaitForSeconds(PER_SPAWN_DELAY_MAX * Random.value);
				FoodObject spawnedObject = foodObjectPools[Random.Range(0, foodObjectPools.Length)].GetPoolObject();
				spawnedObject.Setup(new Vector3(levelWidth * (Random.value - 0.5f), 0, levelHeight * (Random.value - 0.5f)));
			}
		}

		public static FoodObject GetNearestFoodObject(Vector2 point, int planeIndex, float? maxRange = null)
		{
			if (!LevelLoader.GameLevelPlanes[planeIndex].CoreObject)
			{
				return null;
			}

			float shortestDist = (maxRange * maxRange) ?? Mathf.Infinity;
			int targetIndex = -1;
			LevelPlane targetPlane = LevelLoader.GameLevelPlanes[planeIndex].CoreObject;
			List<FoodObject> targetFoodObjectList = targetPlane.AffiliatedFoodSpawner.ActiveFoodObjects;
			for (int i = 0; i < targetFoodObjectList.Count; i++)
			{
				if (!targetFoodObjectList[i] || !targetFoodObjectList[i].isActiveAndEnabled || !targetFoodObjectList[i].IsEdible)
				{
					continue;
				}

				Vector2 foodPos = targetFoodObjectList[i].transform.position.XYZtoXZ();
				if (foodPos.RequiresLevelBoundsClamping(targetPlane.PlaneSettings))
				{
					continue;
				}

				float dist = (foodPos - point).sqrMagnitude;
				if (dist < shortestDist)
				{
					shortestDist = dist;
					targetIndex = i;
				}
			}

			return targetIndex >= 0 ? targetFoodObjectList[targetIndex] : null;
		}
	}
}