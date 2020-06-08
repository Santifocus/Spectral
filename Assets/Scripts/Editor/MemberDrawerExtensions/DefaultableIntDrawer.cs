using Spectral.Runtime.DataStorage;
using UnityEditor;
using UnityEngine;
using static Spectral.Editor.EditorUtils;

namespace Spectral.Editor
{
	public static class DefaultableIntDrawer
	{
		private const float VALUE_LABEL_WIDTH = 55;
		private const float NUMBER_PREFIX_LABEL_WIDTH = 14;

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
				EditorGUILayout.TextArea("(Default)", GUILayout.Width(VALUE_LABEL_WIDTH));
				EditorGUI.EndDisabledGroup();
				int offsetSign = System.Math.Sign(self.DefaultOffset) == -1 ? -1 : 1;
				string offsetSignPrefix = offsetSign                  == -1 ? "-" : "+";
				string numberPrefix = EditorGUILayout.TextField(offsetSignPrefix, GUILayout.Width(NUMBER_PREFIX_LABEL_WIDTH));
				if (numberPrefix != offsetSignPrefix)
				{
					if ((numberPrefix == "+") || (numberPrefix == "-"))
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
				int newValue = EditorGUILayout.IntField(self.NonDefaultValue, GUILayout.Width(VALUE_LABEL_WIDTH));
				if (newValue != self.NonDefaultValue)
				{
					self.NonDefaultValue = newValue;
					ShouldBeDirty();
				}

				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.TextField("/", GUILayout.Width(NUMBER_PREFIX_LABEL_WIDTH));
				EditorGUILayout.TextField("/");
				EditorGUI.EndDisabledGroup();
			}

			EndIndentSpaces();
		}
	}
}