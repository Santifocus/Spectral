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
		private bool visualSettingsOpen;

		protected override bool ShouldHideBaseInspector()
		{
			return true;
		}

		protected override void CustomInspector()
		{
			DrawInFoldout(ref baseSettingsOpen, "Base Settings", DrawBaseSettings, true);
			DrawInFoldout(ref dimensionsSettingsOpen, "Dimensions Settings", DrawDimensionsSettings, true);
			DrawInFoldout(ref foodSettingsOpen, "Food Settings", DrawFoodSettings, true);
			DrawInFoldout(ref visualSettingsOpen, "Visual Settings", DrawVisualSettings, true);
		}

		private void DrawBaseSettings()
		{
			LevelSettings settings = target as LevelSettings;
			IntField(ref settings.LevelSceneID, ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelSceneID)));
			BeginIndentSpaces();
			int targetSceneID = settings.LevelSceneID + 1;
			if ((targetSceneID <= 0) || (targetSceneID >= EditorBuildSettings.scenes.Length))
			{
				EditorGUILayout.HelpBox("This Scene does not Exist!", MessageType.Error);
			}
			else
			{
				EditorBuildSettingsScene scene = EditorBuildSettings.scenes[targetSceneID];
				int subStringStart = scene.path.LastIndexOf("/") + 1;
				int subStringEnd = scene.path.LastIndexOf(".unity");
				EditorGUILayout.LabelField("Target Scene: ", scene.path.Substring(subStringStart, subStringEnd - subStringStart), EditorStyles.boldLabel);
			}

			EndIndentSpaces();
			IntField(ref settings.StartPlayerSize, ObjectNames.NicifyVariableName(nameof(LevelSettings.StartPlayerSize)));
			IntField(ref settings.RequiredPlayerSizeToTransition, ObjectNames.NicifyVariableName(nameof(LevelSettings.RequiredPlayerSizeToTransition)));
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

		private void DrawVisualSettings()
		{
			LevelSettings settings = target as LevelSettings;
			ColorField(ref settings.BackgroundColor, ObjectNames.NicifyVariableName(nameof(LevelSettings.BackgroundColor)), true);
		}
	}
}