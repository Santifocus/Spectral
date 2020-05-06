using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Spectral.EditorInspector.EditorUtils;

namespace Spectral.EditorInspector
{
	public static class DefautableIntDrawer
	{
		private const float VALUE_LABEL_WIDHT = 55;
		private const float NUMBER_PREFIX_LABEL_WIDHT = 14;
		public static void Draw(this ref DefaultableInt self, string label)
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

				int newOffsetValue = EditorGUILayout.DelayedIntField(self.DefaultOffset * offsetSign);

				if ((newOffsetValue * offsetSign) != self.DefaultOffset)
				{
					self.DefaultOffset = newOffsetValue * offsetSign;
					ShouldBeDirty();
				}
			}
			else
			{
				int newValue = EditorGUILayout.IntField(self.RawValue, GUILayout.Width(VALUE_LABEL_WIDHT));
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