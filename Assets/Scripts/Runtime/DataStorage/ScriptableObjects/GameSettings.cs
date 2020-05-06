using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
{
	public class GameSettings : SpectralScriptableObject
	{
		public DefaultEntitySettings DefaultEntitySettings;

		public int AttackablePlayerTorsoCount = 3;
		public float DefaultLevelWidht = 200;
		public float DefaultLevelHeight = 200;
	}
}