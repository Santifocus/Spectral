using Spectral.Runtime.Behaviours;
using UnityEngine;

namespace Spectral.Runtime
{
	public class LevelLoader : SpectralMonoBehavior
	{
		[SerializeField] private LevelSettings[] levels = default;
		private readonly int currentLevelIndex = 0;

		private void Start()
		{
			LevelSettings.SetActiveLevelSettings(levels[currentLevelIndex]);
		}
	}
}