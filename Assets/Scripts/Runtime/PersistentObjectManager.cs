using UnityEngine;

namespace Spectral.Runtime
{
	public class PersistentObjectManager : MonoBehaviour
	{
		public static PersistentObjectManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null)
			{
				gameObject.SetActive(false);
				Destroy(gameObject);
			}
			else
			{
				Initialise();
			}
		}

		private void Initialise()
		{
			Instance = this;
			DontDestroyOnLoad(this);
			InitialiseScreenSettings();
			PersistentDataManager.Initialise();
			SceneChangeManager.ForceSceneIDUpdate();
			PlayerScoreManager.Initialise();
		}

		private static void InitialiseScreenSettings()
		{
#if UNITY_ANDROID || UNITY_IPHONE
			Screen.orientation = ScreenOrientation.Portrait;
#else
			Screen.SetResolution((int) (Screen.height * (9f / 16)), Screen.height, true);
#endif
		}
	}
}