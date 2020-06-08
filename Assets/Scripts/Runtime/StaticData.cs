using System;
using UnityEngine;

namespace Spectral.Runtime
{
	public static class StaticData
	{
		public static bool GameIsPaused { get; private set; }

		public static int ActivePauses
		{
			get => activePauses;
			set
			{
				activePauses = Math.Max(0, value);
				bool newPauseState = activePauses > 0;
				if (newPauseState != GameIsPaused)
				{
					SetPauseGameState(newPauseState);
				}
			}
		}

		private static int activePauses;

		private static void SetPauseGameState(bool state)
		{
			Time.timeScale = state ? 0 : 1;
			GameIsPaused = state;
		}
	}
}