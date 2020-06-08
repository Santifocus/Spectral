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
		}

		private void OnDrawGizmosSelected()
		{
			if (!GameSettings.Current || !LevelSettings.Current)
			{
				return;
			}

			Gizmos.color = Color.cyan / 2;
			Gizmos.DrawCube(Vector3.zero,
							new Vector3(LevelSettings.Current.LevelWidth, 0.5f,
										LevelSettings.Current.LevelHeight));
		}
	}
}