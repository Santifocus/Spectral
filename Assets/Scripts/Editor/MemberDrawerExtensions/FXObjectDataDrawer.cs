using Spectral.Runtime.DataStorage.FX;
using UnityEditor;
using static Spectral.Editor.EditorUtils;

namespace Spectral.Editor.MemberDrawer
{
	public static class FXObjectDataDrawer
	{
		public static void Draw(this FXObjectData self, string label, SerializedProperty property)
		{
			if (self.BaseFX)
			{
				label = ObjectNames.NicifyVariableName(self.BaseFX.name);
			}

			Foldout(property, label);
			if (property.isExpanded)
			{
				IncreaseIndent();
				UnityObjectField<FXObject>(ref self.BaseFX, ObjectNames.NicifyVariableName(nameof(FXObjectData.BaseFX)));
				EnumFlagField<FXDataOverwrite>(ref self.EnabledOverwrites, ObjectNames.NicifyVariableName(nameof(FXObjectData.EnabledOverwrites)));
				self.MultiplierClamps.Draw(ObjectNames.NicifyVariableName(nameof(FXObjectData.MultiplierClamps)),
											property.FindPropertyRelative(nameof(FXObjectData.MultiplierClamps)));

				if (self.EnabledOverwrites.HasFlag(FXDataOverwrite.InitiationDelay))
				{
					FloatField(ref self.InitiationDelay, ObjectNames.NicifyVariableName(nameof(FXObjectData.InitiationDelay)));
				}

				if (self.EnabledOverwrites.HasFlag(FXDataOverwrite.Position))
				{
					Vector3Field(ref self.PositionOffset, ObjectNames.NicifyVariableName(nameof(FXObjectData.PositionOffset)));
				}

				if (self.EnabledOverwrites.HasFlag(FXDataOverwrite.Rotation))
				{
					Vector3Field(ref self.RotationOffset, ObjectNames.NicifyVariableName(nameof(FXObjectData.RotationOffset)));
				}

				if (self.EnabledOverwrites.HasFlag(FXDataOverwrite.Scale))
				{
					Vector3Field(ref self.NewScale, ObjectNames.NicifyVariableName(nameof(FXObjectData.NewScale)));
				}

				DecreaseIndent();
			}
		}
	}
}