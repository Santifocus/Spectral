using Spectral.Runtime.Behaviours.Entities;
using UnityEngine;

namespace Spectral.Runtime.Behaviours.UI
{
	public
		class PlayerDeathScreenController : MonoBehaviour
	{
		[SerializeField] private GameObject deathScreenMainObject = default;

		private void Awake()
		{
			deathScreenMainObject.SetActive(false);
		}

		private void OnEnable()
		{
			PlayerMover.PlayerDied += ShowDeathScreen;
		}

		private void OnDisable()
		{
			PlayerMover.PlayerDied -= ShowDeathScreen;
		}

		public void RestartGame()
		{
			SceneChangeManager.ChangeScene(ConstantCollector.CORE_GAME_SCENE);
		}

		public void BackToMainMenu()
		{
			SceneChangeManager.ChangeScene(ConstantCollector.MAIN_MENU_SCENE);
		}

		private void ShowDeathScreen()
		{
			deathScreenMainObject.SetActive(true);
		}

		private void HideDeathScreen()
		{
			deathScreenMainObject.SetActive(false);
		}
	}
}