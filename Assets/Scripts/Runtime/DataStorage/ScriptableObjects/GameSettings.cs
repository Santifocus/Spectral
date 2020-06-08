using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spectral.Runtime
{
	public class GameSettings : SpectralScriptableObject
	{
		#region Referencing

		private static GameSettings current;
#if UNITY_EDITOR
		public bool ChooseAsEditorReference = true;
		public static GameSettings EditorReference;
		public static GameSettings Current => current == null ? EditorReference : current;
#else
		public static GameSettings Current => current;
#endif
		public static void SetActiveGameSettings(GameSettings target)
		{
			current = target;
		}

		#endregion

		public DefaultEntitySettings DefaultEntitySettings;

		public Material ScreenEffectMaterial;
		public int AttackablePlayerTorsoCount = 3;
		public int DefaultFoodInLevelCount = 10;

		[FormerlySerializedAs("DefaultLevelWidht")]
		public float DefaultLevelWidth = 200;

		public float DefaultLevelHeight = 200;

		public float LevelBorderForceFieldWidth = 3;
		public float LevelBorderForceFieldStrength = 2;

		public FoodObject[] FoodObjectVariants;
	}
}