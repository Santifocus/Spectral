namespace Spectral.Runtime.DataStorage
{
	public class LevelLoaderSettings : SpectralScriptableObject
	{
		public static LevelLoaderSettings Current { get; private set; }

		public static void SetActiveLevelLoaderSettings(LevelLoaderSettings target)
		{
			Current = target;
		}

		public EntitySettings PlayerSettings;
		public LevelSettings[] Levels;
		public int LevelStartIndex = 0;
		public float LevelTransitionTime = 2;
		public float LevelDepth = 20;
	}
}