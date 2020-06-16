using Spectral.Runtime.DataStorage;
using UnityEditor;
using static Spectral.Editor.EditorUtils;

namespace Spectral.Editor
{
	public static class IntMinMaxDrawer
	{
		public static void Draw(this ref IntMinMax self, string label, SerializedProperty property)
		{
			Foldout(property, label);
			if (property.isExpanded)
			{
				IncreaseIndent();
				IntField(ref self.Min, ObjectNames.NicifyVariableName(nameof(IntMinMax.Min)));
				IntField(ref self.Max, ObjectNames.NicifyVariableName(nameof(IntMinMax.Max)));
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