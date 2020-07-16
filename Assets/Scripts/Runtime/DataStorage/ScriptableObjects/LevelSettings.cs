using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime
{
	public class LevelSettings : SpectralScriptableObject
	{
		public int LevelSceneID = 1;
		public int StartPlayerSize = 3;
		public int RequiredPlayerSizeToTransition = 6;

		//Dimensions
		public DefaultableFloat LevelWidth = new DefaultableFloat(() => GameSettings.Current.DefaultLevelWidth);
		public DefaultableFloat LevelHeight = new DefaultableFloat(() => GameSettings.Current.DefaultLevelHeight);

		//Food
		public DefaultableInt FoodInLevel = new DefaultableInt(() => GameSettings.Current.DefaultFoodInLevelCount);
		public FoodObject[] FoodObjectVariants;

		//Background
		public Color BackgroundColor = Color.blue;
		
		//General
		public int MusicIndex = 0;
	}
}