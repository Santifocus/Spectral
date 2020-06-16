using UnityEngine;

namespace Spectral.Runtime
{
	public class StaticDataDeliverer : MonoBehaviour
	{
		[SerializeField] private GameSettings gameSettings = default;

		private void Start()
		{
			GameSettings.SetActiveGameSettings(gameSettings);
			StaticData.ActivePauses = 0;
			LevelLoader.Initialise();
		}

		private void OnDrawGizmosSelected()
		{
			if (!GameSettings.Current || (LevelLoader.GameLevelPlanes == null) || (LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings == null))
			{
				return;
			}

			Gizmos.color = Color.cyan / 2;
			Gizmos.DrawCube(Vector3.zero,
							new Vector3(LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.LevelWidth, 0.5f,
										LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.LevelHeight));
		}
	}
}