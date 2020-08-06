using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;
using Spectral.Runtime.Factories;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Spectral.Runtime
{
	public static class LevelLoader
	{
		private const int TRANSITION_TIME_STEP = 1000 / 120;

		public static Storage CoreStorage { get; private set; }
		public static int PlayerLevelIndex { get; private set; }
		public static LevelPlaneData[] GameLevelPlanes { get; private set; }
		private static Transform LevelPlanesStorage;

		public static event Action<int, LevelPlane, LevelPlane, bool> LevelTransitionBegan;
		public static bool Transitioning { get; private set; }
		private static bool UnloadingScene => UnloadingSceneCount > 0;
		private static int UnloadingSceneCount;

		private static MusicInstance lastMusicInstance;

		public static async void Initialise()
		{
			//Basic setups
			InitialiseCoreStorage();
			InitialiseGamePlaneArray();
			PlayerLevelIndex = LevelLoaderSettings.Current.LevelStartIndex;

			//Reset Score manager
			PlayerScoreManager.Reset();

			//Spawn the player
			EntityFactory.CreatePlayerEntity();

			//Create all required Level planes
			await CreateLevelPlane(PlayerLevelIndex, 0);
			await UnloadSceneTask();
			await CreateLevelPlane(PlayerLevelIndex + 1, 1);
			await UnloadSceneTask();
			await CreateLevelPlane(PlayerLevelIndex - 1, -1);
			await UnloadSceneTask();

			//Setup the music for the plane
			lastMusicInstance = new MusicInstance(0, 0, GameLevelPlanes[PlayerLevelIndex].PlaneSettings.MusicIndex);
			MusicController.Instance.AddMusicInstance(lastMusicInstance);

			//Event subscription
			SceneChangeManager.GameSceneWillExit += TearDown;
			TransitionGate.PlayerWantsToStartLevelTransition += TransitionLevel;
		}

		private static void TearDown()
		{
			SceneChangeManager.GameSceneWillExit -= TearDown;
			TransitionGate.PlayerWantsToStartLevelTransition -= TransitionLevel;
		}

		private static void InitialiseCoreStorage()
		{
			CoreStorage = new Storage(null);
			LevelPlanesStorage = new GameObject("Level Planes").transform;
			LevelPlanesStorage.SetParent(CoreStorage.Main);
		}

		private static void InitialiseGamePlaneArray()
		{
			GameLevelPlanes = new LevelPlaneData[LevelLoaderSettings.Current.Levels.Length];
			for (int i = 0; i < GameLevelPlanes.Length; i++)
			{
				GameLevelPlanes[i] = new LevelPlaneData(LevelLoaderSettings.Current.Levels[i]);
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
			GameLevelPlanes[targetLevelIndex].CoreObject = planeCore.gameObject.AddComponent<LevelPlane>();
			GameLevelPlanes[targetLevelIndex].CoreObject.Initialise(targetLevelIndex);

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
				throw new SystemException($"Tried to load Scene via {nameof(LevelLoader)} while in non play-mode.");
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
			UnloadScene(targetScene);

			return extractedObjectParent;
		}

		private static async void UnloadScene(Scene targetScene)
		{
			UnloadingSceneCount++;
			AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(targetScene);
			while (!unloadOperation.isDone)
			{
				await Task.Delay(1);
			}

			UnloadingSceneCount--;
		}

		private static async Task UnloadSceneTask()
		{
			while (UnloadingScene)
			{
				await Task.Delay(1);
			}
		}

		private static async void TransitionLevel(int direction, bool hasTransitionedToPlaneBefore)
		{
			int targetLevelIndex = PlayerLevelIndex + direction;
			if (Transitioning || (targetLevelIndex < 0) || (targetLevelIndex >= LevelLoaderSettings.Current.Levels.Length))
			{
				return;
			}

			Transitioning = true;
			await CreateLevelPlane(PlayerLevelIndex + (direction * 2), direction * 2);

			//Call Transition event and update the PlayerLevelIndex
			LevelTransitionBegan?.Invoke(direction,
										GameLevelPlanes[PlayerLevelIndex].CoreObject,
										GameLevelPlanes[PlayerLevelIndex = targetLevelIndex].CoreObject,
										hasTransitionedToPlaneBefore);

			//Update the music track
			lastMusicInstance.WantsToPlay = false;
			lastMusicInstance = new MusicInstance(0, 0, GameLevelPlanes[PlayerLevelIndex = targetLevelIndex].PlaneSettings.MusicIndex);
			MusicController.Instance.AddMusicInstance(lastMusicInstance);
			(LevelPlaneData planeData, float prevTransitionDepth)[] targetObjectData = GameLevelPlanes.Where(p => p.CoreObject).Select(p => (p, p.CurrentPlaneDepth)).ToArray();
			float startTime = Time.time;
			float currentTime = 0;
			while (currentTime < LevelLoaderSettings.Current.LevelTransitionTime)
			{
				int currentFrame = Time.frameCount;
				while (currentFrame == Time.frameCount)
				{
					await Task.Delay(1);
				}

				currentTime = Time.time - startTime;
				float transitionPoint = currentTime / LevelLoaderSettings.Current.LevelTransitionTime;
				foreach ((LevelPlaneData planeData, float prevTransitionDepth) in targetObjectData)
				{
					planeData.CurrentPlaneDepth = prevTransitionDepth - (transitionPoint * direction);
				}
			}

			foreach ((LevelPlaneData planeData, float prevTransitionDepth) in targetObjectData)
			{
				planeData.CurrentPlaneDepth = prevTransitionDepth - direction;
				planeData.CheckForClearing();
			}

			Transitioning = false;
		}

		public class LevelPlaneData
		{
			public LevelPlane CoreObject;
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

			public LevelPlaneData(LevelSettings planeSettings)
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