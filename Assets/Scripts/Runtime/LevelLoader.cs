using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spectral.Runtime
{
	public static class LevelLoader
	{
		private const int TRANSITION_TIME_STEP = 1000 / 120;

		public static Storage CoreStorage { get; private set; }
		public static int PlayerLevelIndex { get; private set; }
		public static LevelPlane[] GameLevelPlanes { get; private set; }
		private static Transform LevelPlanesStorage;

		private static bool Transitioning;

		public static async void Initiate()
		{
			//Basic setups
			InitiateCoreStorage();
			InitiateGamePlaneArray();
			PlayerLevelIndex = LevelLoaderSettings.Current.LevelStartIndex;

			//Create all required Level planes
			await Task.WhenAll(CreateLevelPlane(PlayerLevelIndex - 1, -1),
								CreateLevelPlane(PlayerLevelIndex, 0),
								CreateLevelPlane(PlayerLevelIndex + 1, 1));

			//Spawn the player
			EntityFactory.CreatePlayerEntity();
		}

		private static void InitiateCoreStorage()
		{
			CoreStorage = new Storage(null);
			LevelPlanesStorage = new GameObject("Level Planes").transform;
			LevelPlanesStorage.SetParent(CoreStorage.Main);
		}

		private static void InitiateGamePlaneArray()
		{
			GameLevelPlanes = new LevelPlane[LevelLoaderSettings.Current.Levels.Length];
			for (int i = 0; i < GameLevelPlanes.Length; i++)
			{
				GameLevelPlanes[i] = new LevelPlane(LevelLoaderSettings.Current.Levels[i]);
			}
		}

		private static async Task CreateLevelPlane(int targetLevelIndex, int creationDepth)
		{
			if ((targetLevelIndex < 0) || (targetLevelIndex >= LevelLoaderSettings.Current.Levels.Length) || GameLevelPlanes[targetLevelIndex].CoreObject)
			{
				return;
			}

			Transform planeCore = await ExtractLevelPlaneObjects(targetLevelIndex);

			//Setup the Plane Level Data
			GameLevelPlanes[targetLevelIndex].CoreObject = planeCore.gameObject.AddComponent<PlaneLevelData>();
			GameLevelPlanes[targetLevelIndex].CoreObject.Initiate(targetLevelIndex);

			//Update the depth
			GameLevelPlanes[targetLevelIndex].CurrentPlaneDepth = creationDepth;
		}

		private static async Task<Transform> ExtractLevelPlaneObjects(int levelIndex)
		{
			int levelSceneID = LevelLoaderSettings.Current.Levels[levelIndex].LevelSceneID;

			//Load the scene
			SceneManager.LoadScene(levelSceneID, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.None));
			Scene targetScene = SceneManager.GetSceneByBuildIndex(levelSceneID);
			Transform extractedObjectParent = new GameObject($"{targetScene.name}s Objects").transform;
			extractedObjectParent.SetParent(LevelPlanesStorage);
			while (!targetScene.isLoaded)
			{
				await Task.Delay(1);
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				throw new System.SystemException($"Tried to load Scene via {nameof(LevelLoader)} while in non play-mode.");
			}
#endif

			//Extract all root objects from the level into a parent object so we have full control over it
			List<GameObject> levelRootObjects = new List<GameObject>();
			targetScene.GetRootGameObjects(levelRootObjects);
			foreach (GameObject rootObject in levelRootObjects)
			{
				rootObject.transform.SetParent(extractedObjectParent);
			}

			//Now unload the empty scene
			SceneManager.UnloadSceneAsync(targetScene);

			return extractedObjectParent;
		}

		private static async void TransitionLevel(int direction)
		{
			int targetLevelIndex = PlayerLevelIndex + direction;
			if (Transitioning || (targetLevelIndex < 0) || (targetLevelIndex >= LevelLoaderSettings.Current.Levels.Length))
			{
				return;
			}

			Transitioning = true;
			await CreateLevelPlane(PlayerLevelIndex + (direction * 2), direction * 2);
			PlayerLevelIndex = targetLevelIndex;
			(LevelPlane levelPlane, float prevTransitionDepth)[] targetObjectData = GameLevelPlanes.Where(p => p.CoreObject).Select(p => (p, p.CurrentPlaneDepth)).ToArray();
			int millisecondsPassed = 0;
			float totalMilliseconds = LevelLoaderSettings.Current.LevelTransitionTime * 1000;
			while (millisecondsPassed < totalMilliseconds)
			{
				await Task.Delay(TRANSITION_TIME_STEP);
				millisecondsPassed += TRANSITION_TIME_STEP;
				float transitionPoint = millisecondsPassed / totalMilliseconds;
				foreach ((LevelPlane levelPlane, float prevTransitionDepth) in targetObjectData)
				{
					levelPlane.CurrentPlaneDepth = prevTransitionDepth - (transitionPoint * direction);
				}
			}

			foreach ((LevelPlane levelPlane, float prevTransitionDepth) in targetObjectData)
			{
				levelPlane.CurrentPlaneDepth = prevTransitionDepth - direction;
				levelPlane.CheckForClearing();
			}

			Transitioning = false;
		}

		public class LevelPlane
		{
			public PlaneLevelData CoreObject;
			public readonly LevelSettings PlaneSettings;
			private float currentPlaneDepth;

			public float CurrentPlaneDepth
			{
				get => currentPlaneDepth;
				set
				{
					currentPlaneDepth = value;
					if (CoreObject)
					{
						CoreObject.transform.position = Vector3.down * (currentPlaneDepth * LevelLoaderSettings.Current.LevelDepth);
					}
				}
			}

			public LevelPlane(LevelSettings planeSettings)
			{
				PlaneSettings = planeSettings;
			}

			public void CheckForClearing()
			{
				if ((CurrentPlaneDepth <= -1.999f) || (CurrentPlaneDepth >= 1.999f))
				{
					Object.Destroy(CoreObject.gameObject);
				}
			}
		}
	}
}