using Spectral.Runtime;
using UnityEditor;

namespace Spectral.Editor
{
	[CustomEditor(typeof(LevelSettings), true)]
	public class LevelSettingsEditor : ObjectEditor
	{
		protected override bool ShouldHideBaseInspector()
		{
			return true;
		}

		protected override void CustomInspector()
		{
			LevelSettings settings = target as LevelSettings;
			settings.LevelWidth.Draw(ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelWidth)));
			settings.LevelHeight.Draw(ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelHeight)));
		}
	}
}