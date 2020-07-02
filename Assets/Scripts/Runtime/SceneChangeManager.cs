using UnityEngine.SceneManagement;

namespace Spectral.Runtime
{
	public static class SceneChangeManager
	{
		public static void ChangeScene(int sceneID)
		{
			SceneManager.LoadScene(sceneID);
		}
	}
}