using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;

namespace Spectral.Runtime
{
	public class LevelSettings : SpectralScriptableObject
	{
		public int LevelSceneID = 1;

		//Dimensions
		public DefaultableFloat LevelWidth = new DefaultableFloat(() => GameSettings.Current.DefaultLevelWidth);
		public DefaultableFloat LevelHeight = new DefaultableFloat(() => GameSettings.Current.DefaultLevelHeight);

		//Food
		public DefaultableInt FoodInLevel = new DefaultableInt(() => GameSettings.Current.DefaultFoodInLevelCount);
		public FoodObject[] FoodObjectVariants;
	}
}