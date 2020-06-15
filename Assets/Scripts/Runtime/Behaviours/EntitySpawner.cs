using System.Collections.Generic;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class EntitySpawner : LevelPlaneBehavior
	{
		[SerializeField] private SpawnableEntity targetSpawnable;
		[SerializeField] private int maxSpawnCount = 3;
		[SerializeField] private float spawnRange = 10;
		[SerializeField] private float spawnDelayMin = 0;
		[SerializeField] private float spawnDelayMax = 3;

		private readonly List<MonoBehaviour> createdEntities = new List<MonoBehaviour>();
		private float spawnCooldown;

		private void Update()
		{
			if (spawnCooldown > 0)
			{
				spawnCooldown -= Time.deltaTime;

				return;
			}

			ValidateEntityList();
			CheckForSpawning();
		}

		private void ValidateEntityList()
		{
			for (int i = 0; i < createdEntities.Count; i++)
			{
				if (createdEntities[i] == null)
				{
					createdEntities.RemoveAt(i);
					i--;
				}
			}
		}

		private void CheckForSpawning()
		{
			if (!PlaneLevelIndex.HasValue)
			{
				return;
			}

			if (createdEntities.Count < maxSpawnCount)
			{
				float spawnDirectionAngle = Random.value * 360;
				Vector2 spawnPos = transform.position.XYZtoXZ() + (new Vector2(Mathf.Cos(spawnDirectionAngle * Mathf.Deg2Rad), Mathf.Sin(spawnDirectionAngle * Mathf.Deg2Rad)) *
																	(Random.value * spawnRange));

				createdEntities.Add(targetSpawnable.Spawn(spawnPos, PlaneLevelIndex.Value));
				spawnCooldown = Random.Range(spawnDelayMin, spawnDelayMax);
			}
		}
	}
}