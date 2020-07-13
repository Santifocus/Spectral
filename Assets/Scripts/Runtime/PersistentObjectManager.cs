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
			PersistentDataManager.Initialise();
			SceneChangeManager.ForceSceneIDUpdate();
			PlayerScoreManager.Initialise();
		}
	}
}