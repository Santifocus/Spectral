using UnityEngine;

namespace Spectral.Runtime
{
	public class PersistentObjectManager : MonoBehaviour
	{
		private static bool Initiated;

		private void Awake()
		{
			if (Initiated)
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
			Initiated = true;
			PersistentSettingsManager.ReadOrCreate();
			DontDestroyOnLoad(this);
		}
	}
}