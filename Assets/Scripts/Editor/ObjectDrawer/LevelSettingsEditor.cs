using Spectral.Behaviours.Entities;
using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Spectral.EditorInspector.EditorUtils;

namespace Spectral.EditorInspector
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
			settings.LevelWidht.Draw(ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelWidht)));
			settings.LevelHeight.Draw(ObjectNames.NicifyVariableName(nameof(LevelSettings.LevelHeight)));
		}
	}
}