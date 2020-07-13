using System;
using UnityEngine.SceneManagement;

namespace Spectral.Runtime
{
	public static class SceneChangeManager
	{
		private static int CurrentSceneID;
		public static event Action GameSceneWillExit;

		public static void ChangeScene(int sceneID)
		{
			if (CurrentSceneID == ConstantCollector.CORE_GAME_SCENE)
			{
				GameSceneWillExit?.Invoke();
			}

			CurrentSceneID = sceneID;
			SceneManager.LoadScene(sceneID);
		}

		public static void ForceSceneIDUpdate()
		{
			CurrentSceneID = SceneManager.GetActiveScene().buildIndex;
		}
	}
}