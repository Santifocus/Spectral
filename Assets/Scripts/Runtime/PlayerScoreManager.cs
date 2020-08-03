using System;
using System.Collections;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.DataStorage;
using Spectral.Runtime.Factories;
using UnityEngine;

namespace Spectral.Runtime
{
	public static class PlayerScoreManager
	{
		public static int CurrentPlayerScore { get; private set; }
		public static event Action PlayerScoreChanged;

		public static void Initialise()
		{
			Application.quitting += TearDown;
			LevelLoader.LevelTransitionBegan += StartedLevelTransition;
			PlayerMover.PlayerDied += PlayerDied;
		}

		private static void TearDown()
		{
			Application.quitting -= TearDown;
			LevelLoader.LevelTransitionBegan -= StartedLevelTransition;
			PlayerMover.PlayerDied -= PlayerDied;
		}

		public static void Reset()
		{
			CurrentPlayerScore = 0;
			PlayerScoreChanged?.Invoke();
		}

		private static void PlayerDied()
		{
			if (CurrentPlayerScore > PersistentDataManager.CurrentPlayerData.HighestScore)
			{
				PersistentDataManager.CurrentPlayerData.HighestScore = CurrentPlayerScore;
				PersistentDataManager.SaveOrCreatePlayerData();
			}
		}

		public static void ChangePlayerScore(int change)
		{
			CurrentPlayerScore += change;
			PlayerScoreChanged?.Invoke();
		}

		private static void StartedLevelTransition(int direction, LevelPlane previousLevelPlane, LevelPlane newLevelPlane, bool hasTransitionedToPlaneBefore)
		{
			PersistentObjectManager.Instance.StartCoroutine(StartedLevelTransitionCoroutine(direction, previousLevelPlane, newLevelPlane, hasTransitionedToPlaneBefore));
		}

		private static IEnumerator StartedLevelTransitionCoroutine(int direction, LevelPlane previousLevelPlane, LevelPlane newLevelPlane, bool hasTransitionedToPlaneBefore)
		{
			if (direction == -1)
			{
				yield break;
			}

			int playerPartsToRemoveCount = EntityFactory.GetEntitySize(PlayerMover.Instance) - newLevelPlane.PlaneSettings.StartPlayerSize;
			if (playerPartsToRemoveCount <= 0)
			{
				yield break;
			}

			float delayPerRemove = LevelLoaderSettings.Current.LevelTransitionTime / (playerPartsToRemoveCount + 2);
			for (int i = 0; i < playerPartsToRemoveCount; i++)
			{
				float removeDelay = delayPerRemove;
				while (removeDelay > 0)
				{
					yield return new WaitForEndOfFrame();
					removeDelay -= Time.deltaTime;
				}

				if (!PlayerMover.Existent)
				{
					yield break;
				}

				PlayerMover.Instance.Damage(1, true);
				if (!hasTransitionedToPlaneBefore)
				{
					ChangePlayerScore(1);
				}
			}
		}
	}
}