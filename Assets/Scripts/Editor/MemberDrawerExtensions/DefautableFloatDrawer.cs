using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Spectral.EditorInspector.EditorUtils;

namespace Spectral.EditorInspector
{
	public static class DefautableFloatDrawer
	{
		private const float VALUE_LABEL_WIDHT = 55;
		private const float NUMBER_PREFIX_LABEL_WIDHT = 14;
		public static void Draw(this ref DefaultableFloat self, string label)
		{
			BeginIndentSpaces();
			bool newUseDefault = EditorGUILayout.Toggle(label, self.UseDefault);

			if (newUseDefault != self.UseDefault)
			{
				self.UseDefault = newUseDefault;
				ShouldBeDirty();
			}
			if (self.UseDefault)
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.TextArea("(Default)", GUILayout.Width(VALUE_LABEL_WIDHT));
				EditorGUI.EndDisabledGroup();

				int offsetSign = System.Math.Sign(self.DefaultOffset) == -1 ? -1 : 1;
				string offsetSignPrefix = offsetSign == -1 ? "-" : "+";
				string numberPrefix = EditorGUILayout.TextField(offsetSignPrefix, GUILayout.Width(NUMBER_PREFIX_LABEL_WIDHT));

				if (numberPrefix != offsetSignPrefix)
				{
					if (numberPrefix == "+" || numberPrefix == "-")
					{
						self.DefaultOffset *= -1;
						offsetSign *= -1;
					}
				}

				float newOffsetValue = EditorGUILayout.DelayedFloatField(self.DefaultOffset * offsetSign);

				if ((newOffsetValue * offsetSign) != self.DefaultOffset)
				{
					self.DefaultOffset = newOffsetValue * offsetSign;
					ShouldBeDirty();
				}
			}
			else
			{
				float newValue = EditorGUILayout.FloatField(self.RawValue, GUILayout.Width(VALUE_LABEL_WIDHT));
				if (newValue != self.RawValue)
				{
					self.RawValue = newValue;
					ShouldBeDirty();
				}
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.TextField("/", GUILayout.Width(NUMBER_PREFIX_LABEL_WIDHT));
				EditorGUILayout.TextField("/");
				EditorGUI.EndDisabledGroup();
			}
			EndIndentSpaces();
		}
	}
}