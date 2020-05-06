using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }
		public static GameSettings CurrentGameSettings => Instance.gameSettings;
		public static LevelSettings CurrentLevelSettings => Instance.levelSettings;

		[SerializeField] private GameSettings gameSettings = default;
		[SerializeField] private LevelSettings levelSettings = default;
		private void Start()
		{
			Instance = this;
		}
		private void OnDrawGizmosSelected()
		{
			if (!gameSettings || !levelSettings)
			{
				return;
			}

			Gizmos.color = Color.cyan / 2;
			Gizmos.DrawCube(Vector3.zero, new Vector3(levelSettings.LevelWidht.GetDefaultedValue(gameSettings.DefaultLevelWidht), 0.5f, levelSettings.LevelHeight.GetDefaultedValue(gameSettings.DefaultLevelHeight)));
		}
	}
}