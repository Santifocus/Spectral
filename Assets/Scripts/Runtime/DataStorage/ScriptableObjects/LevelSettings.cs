using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
{
	public class LevelSettings : SpectralScriptableObject
	{
		public DefaultableInt FoodInLevel = new DefaultableInt(null);
		public DefaultableFloat LevelWidht = new DefaultableFloat(null);
		public DefaultableFloat LevelHeight = new DefaultableFloat(null);
	}
}