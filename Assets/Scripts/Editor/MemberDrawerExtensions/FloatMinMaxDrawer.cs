using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Spectral.EditorInspector.EditorUtils;

namespace Spectral.EditorInspector
{
	public static class FloatMinMaxDrawer
	{
		public static void Draw(this ref FloatMinMax self, string label, SerializedProperty property)
		{
			Foldout(label, property);
			if (property.isExpanded)
			{
				IncreaseIndent();
				FloatField(ObjectNames.NicifyVariableName(nameof(IntMinMax.Min)), ref self.Min);
				FloatField(ObjectNames.NicifyVariableName(nameof(IntMinMax.Max)), ref self.Max);
				if (self.Min == self.Max)
				{
					BeginIndentSpaces();
					EditorGUILayout.HelpBox("Always " + self.Min, MessageType.None);
					EndIndentSpaces();
				}
				DecreaseIndent();
			}
			if (self.Min > self.Max)
			{
				self.Max = self.Min;
				ShouldBeDirty();
			}
			if (self.Max < self.Min)
			{
				self.Min = self.Max;
				ShouldBeDirty();
			}
		}
	}
}