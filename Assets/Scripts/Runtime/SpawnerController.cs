using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral
{
	public class SpawnerController : MonoBehaviour
	{
		[SerializeField] private GameObject toSpawn = default;
		[SerializeField] private Transform focus = default;
		[SerializeField] private float distanceMin = 5;
		[SerializeField] private float distanceMax = 19;
		[SerializeField] private int amountMin = 1;
		[SerializeField] private int amountMax = 3;
		[SerializeField] private float delayPerSpawnIterationMin = 2;
		[SerializeField] private float delayPerSpawnIterationMax = 7;

		private float tillNextSpawn;

		private void Start()
		{
			ResetDelayTimer();
		}
		private void Update()
		{
			tillNextSpawn -= Time.deltaTime;
			if(tillNextSpawn <= 0)
			{
				SpawnObjects();
			}
		}
		private void ResetDelayTimer()
		{
			tillNextSpawn = Random.Range(delayPerSpawnIterationMin, delayPerSpawnIterationMax);
		}
		private void SpawnObjects()
		{
			ResetDelayTimer();
			int amount = Random.Range(amountMin, amountMax);
			for(int i = 0; i < amount; i++)
			{
				float distance = Random.Range(distanceMin, distanceMax);
				float angle = Random.Range(0, 359);
				Vector3 offsetDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

				Instantiate(toSpawn, focus.position + offsetDirection * distance, Quaternion.Euler(0, Random.Range(0, 359), 0));
			}
		}
	}
}
