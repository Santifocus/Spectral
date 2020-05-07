using Spectral.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
{
	public class GameSettings : SpectralScriptableObject
	{
		public DefaultEntitySettings DefaultEntitySettings;

		public int AttackablePlayerTorsoCount = 3;
		public int DefaultFoodInLevelCount = 10;
		public float DefaultLevelWidht = 200;
		public float DefaultLevelHeight = 200;

		public FoodObject[] FoodObjectVariants;
	}
}