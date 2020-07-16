using UnityEngine;

namespace Spectral.Runtime.Behaviours.UI
{
	public class PauseScreenController : MonoBehaviour
	{
		[SerializeField] private GameObject pauseScreenMainObject = default;
		[SerializeField] private GameObject settingsScreenMainObject = default;
		[SerializeField] private SettingsUIController settingsUIController = default;
		private bool currentPauseScreenState;

		private void Start()
		{
			SetSettingsActive(false);
			pauseScreenMainObject.SetActive(false);
			settingsUIController.SettingsMenuWantsToClose += SettingsMenuWantsToClose;
		}

		private void OnDestroy()
		{
			settingsUIController.SettingsMenuWantsToClose -= SettingsMenuWantsToClose;
		}

		public void SetPauseScreenState(bool state)
		{
			if (state == currentPauseScreenState)
			{
				return;
			}

			currentPauseScreenState = state;
			pauseScreenMainObject.SetActive(state);
			StaticData.ActivePauses += state ? 1 : -1;
		}

		public void BackToMainMenu()
		{
			SceneChangeManager.ChangeScene(ConstantCollector.MAIN_MENU_SCENE);
		}

		private void SettingsMenuWantsToClose()
		{
			SetSettingsActive(false);
		}

		public void SetSettingsActive(bool state)
		{
			pauseScreenMainObject.SetActive(!state);
			settingsScreenMainObject.SetActive(state);
			if (state)
			{
				settingsUIController.Initialise();
			}
		}

		public void QuitGame()
		{
			Utils.QuitGame();
		}
	}
}