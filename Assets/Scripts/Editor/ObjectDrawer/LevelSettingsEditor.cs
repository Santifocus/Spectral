using Spectral.Runtime;
using Spectral.Runtime.Behaviours;
using UnityEditor;
using static Spectral.Editor.EditorUtils;

namespace Spectral.Editor
{
	[CustomEditor(typeof(LevelSettings), true)]
	public class LevelSettingsEditor : ObjectEditor
	{
		private bool baseSettingsOpen;
		private bool dimensionsSettingsOpen;
		private bool foodSettingsOpen;

		protected override bool ShouldHideBaseInspector()
		{
			return true;
		}

		protected override void CustomInspector()
		{
			DrawInFoldout(ref baseSettingsOpen, "Base Settings", DrawBaseSettings, true);
			DrawInFoldout(ref dimensionsSettingsOpen, "Dimensions Settings", DrawDimensionsSettings, true);
			DrawInFoldout(ref foodSettingsOpen, "Food Settings", DrawFoodSettings, true);
		}

		private void DrawBaseSettings()
		{
			LevelSettings settings = target as LevelSettings;
			IntField(ref settings.LevelSceneID, ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelSceneID)));
		}

		private void DrawDimensionsSettings()
		{
			LevelSettings settings = target as LevelSettings;
			settings.LevelWidth.Draw(ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelWidth)));
			settings.LevelHeight.Draw(ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelHeight)));
		}

		private void DrawFoodSettings()
		{
			LevelSettings settings = target as LevelSettings;
			settings.FoodInLevel.Draw(ObjectNames.NicifyVariableName(nameof(LevelSettings.FoodInLevel)));
			DrawUnityObjectArray<FoodObject>(ref settings.FoodObjectVariants, ObjectNames.NicifyVariableName(nameof(LevelSettings.FoodObjectVariants)),
											serializedObject.FindProperty(nameof(LevelSettings.FoodObjectVariants)), ArrayDrawStyle.Default);
		}
	}
}