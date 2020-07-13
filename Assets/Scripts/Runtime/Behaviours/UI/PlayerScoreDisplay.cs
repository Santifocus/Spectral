using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.Factories;
using TMPro;
using UnityEngine;

namespace Spectral.Runtime.Behaviours.UI
{
	public class PlayerScoreDisplay : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI playerScoreDisplay = default;
		[SerializeField] private TextMeshProUGUI playerSizeDisplay = default;

		private void Start()
		{
			PlayerScoreManager.PlayerScoreChanged += UpdateScoreDisplay;
			PlayerMover.PlayerEntityChangedSize += UpdateSizeDisplay;
			UpdateScoreDisplay();
			UpdateSizeDisplay();
		}

		private void OnDestroy()
		{
			PlayerScoreManager.PlayerScoreChanged -= UpdateScoreDisplay;
			PlayerMover.PlayerEntityChangedSize -= UpdateSizeDisplay;
		}

		private void UpdateScoreDisplay()
		{
			playerScoreDisplay.text = PlayerScoreManager.CurrentPlayerScore.ToString();
		}

		private void UpdateSizeDisplay()
		{
			playerSizeDisplay.text =
				$"{GetPlayerSize().ToString()} / {GetRequiredPlayerSize().ToString()}";
		}

		private int GetPlayerSize()
		{
			return !PlayerMover.Existent ? 0 : EntityFactory.GetEntitySize(PlayerMover.Instance);
		}

		private int GetRequiredPlayerSize()
		{
			return LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.RequiredPlayerSizeToTransition;
		}
	}
}