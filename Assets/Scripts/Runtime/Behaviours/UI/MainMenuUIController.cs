using System;
using UnityEngine;

namespace Spectral.Runtime.Behaviours.UI
{
	public class MainMenuUIController : MonoBehaviour
	{
		[SerializeField] private GameObject mainUIObject = default;
		[SerializeField] private GameObject settingsUIObject = default;
		[SerializeField] private SettingsUIController settingsUIController = default;

		private void Start()
		{
			SetSettingsActive(false);
			settingsUIController.SettingsMenuWantsToClose += SettingsMenuWantsToClose;
		}

		private void OnDestroy()
		{
			settingsUIController.SettingsMenuWantsToClose -= SettingsMenuWantsToClose;
		}

		public void StartGame()
		{
			SceneChangeManager.ChangeScene(ConstantCollector.CORE_GAME_SCENE);
		}

		private void SettingsMenuWantsToClose()
		{
			SetSettingsActive(false);
		}
		
		public void SetSettingsActive(bool state)
		{
			mainUIObject.SetActive(!state);
			settingsUIObject.SetActive(state);
			if (state)
			{
				settingsUIController.Initialise();
			}
		}

		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}