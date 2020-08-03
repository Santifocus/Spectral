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
			playerScoreDisplay.text = (PlayerScoreManager.CurrentPlayerScore * ConstantCollector.SCORE_MULTIPLIER).ToString();
		}

		private void UpdateSizeDisplay()
		{
			playerSizeDisplay.text =
				$"{GetPlayerSize().ToString()} / {GetRequiredPlayerSize().ToString()}";
		}

		private int GetPlayerSize()
		{
			return !PlayerMover.Existent ? 0 : EntityFactory.GetEntitySize(PlayerMover.Instance) - PlayerMinSize();
		}

		private int GetRequiredPlayerSize()
		{
			return !PlayerMover.Existent ? 0 : LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.RequiredPlayerSizeToTransition - PlayerMinSize();
		}

		private int PlayerMinSize()
		{
			return PlayerMover.Instance.EntitySettings.MinParts;
		}
	}
}