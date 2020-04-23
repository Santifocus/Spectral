using Spectral.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }
		public static GameSettings CurrentGameSettings => Instance.gameSettings;

		[SerializeField] private GameSettings gameSettings = default;
		private void Start()
		{
			Instance = this;
		}
	}
}