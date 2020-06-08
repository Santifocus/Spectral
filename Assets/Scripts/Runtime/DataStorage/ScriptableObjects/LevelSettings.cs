using Spectral.Runtime.DataStorage;

namespace Spectral.Runtime
{
	public class LevelSettings : SpectralScriptableObject
	{
		#region Referencing

		public static LevelSettings Current { get; private set; }

		public static void SetActiveLevelSettings(LevelSettings target)
		{
			Current = target;
		}

		#endregion

		public DefaultableInt FoodInLevel = new DefaultableInt(() => GameSettings.Current.DefaultFoodInLevelCount);
		public DefaultableFloat LevelWidth = new DefaultableFloat(() => GameSettings.Current.DefaultLevelWidth);
		public DefaultableFloat LevelHeight = new DefaultableFloat(() => GameSettings.Current.DefaultLevelHeight);
	}
}