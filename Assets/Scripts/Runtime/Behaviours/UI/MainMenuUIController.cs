using TMPro;
using UnityEngine;

namespace Spectral.Runtime.Behaviours.UI
{
	public class MainMenuUIController : MonoBehaviour
	{
		[SerializeField] private int mainMenuMusicIndex = 0;
		[SerializeField] private GameObject mainUIObject = default;
		[SerializeField] private GameObject settingsUIObject = default;
		[SerializeField] private GameObject creditsUIObject = default;
		[SerializeField] private SettingsUIController settingsUIController = default;

		[Header("High Score Display")] [SerializeField]
		private TextMeshProUGUI highScoreDisplay = default;

		[SerializeField] private string highScoreTextPrefix = "Your High-Score: ";

		private void Start()
		{
			SetSettingsActive(false);
			settingsUIController.SettingsMenuWantsToClose += SettingsMenuWantsToClose;
			highScoreDisplay.text = $"{highScoreTextPrefix}{PersistentDataManager.CurrentPlayerData.HighestScore.ToString()}";
			MusicController.Instance.AddMusicInstance(new MusicInstance(0, 0, mainMenuMusicIndex));
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

		public void SetCreditsActive(bool state)
		{
			mainUIObject.SetActive(!state);
			creditsUIObject.SetActive(state);
		}

		public void QuitGame()
		{
			Utils.QuitGame();
		}
	}
}