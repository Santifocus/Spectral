using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.DataStorage;
using UnityEditor;
using static Spectral.Editor.EditorUtils;

namespace Spectral.Editor
{
	[CustomEditor(typeof(EntitySettings), true)]
	public class EntitySettingsEditor : ObjectEditor
	{
		private bool movementSettingsOpen;
		private bool bodySettingsOpen;
		private bool otherSettingsOpen;

		protected override bool ShouldHideBaseInspector()
		{
			return true;
		}

		protected override void CustomInspector()
		{
			DrawInFoldout(ref movementSettingsOpen, "Movement Settings", DrawMovementSettings, true);
			DrawInFoldout(ref bodySettingsOpen, "Body Settings", DrawBodySettings, true);
			DrawInFoldout(ref otherSettingsOpen, "Other Settings", DrawOtherSettings, true);
		}

		protected virtual void DrawMovementSettings()
		{
			EntitySettings settings = target as EntitySettings;
			IncreaseIndent();
			Header("Speed");
			settings.MoveSpeed.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.MoveSpeed)));
			settings.VelocityDamping.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.VelocityDamping)));
			Header("Acceleration");
			settings.Acceleration.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.Acceleration)));
			settings.Deceleration.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.Deceleration)));
			settings.AccelerationAngle.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.AccelerationAngle)));
			Header("Turning");
			settings.TurnSpeed.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.TurnSpeed)));
			settings.TurnAcceleration.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.TurnAcceleration)));
			settings.TurnSmoothAngle.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.TurnSmoothAngle)));
			settings.TurnSmoothMultiplier.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.TurnSmoothMultiplier)));
			DecreaseIndent();
		}

		protected virtual void DrawBodySettings()
		{
			EntitySettings settings = target as EntitySettings;
			IncreaseIndent();
			IntField(ref settings.MinParts, ObjectNames.NicifyVariableName(nameof(EntitySettings.MinParts)));
			settings.PartMinimumScale.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.PartMinimumScale)));
			settings.ScaleChangePerPart.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.ScaleChangePerPart)));
			UnityObjectField<EntityBodyPartConfiguration>(ref settings.EntityHead, ObjectNames.NicifyVariableName(nameof(EntitySettings.EntityHead)));
			EnumField<ChooseStyle>(ref settings.TorsoChooseStyle, ObjectNames.NicifyVariableName(nameof(EntitySettings.TorsoChooseStyle)));
			DrawUnityObjectArray<EntityBodyPartConfiguration>(ref settings.EntityTorso, ObjectNames.NicifyVariableName(nameof(EntitySettings.EntityTorso)),
															serializedObject.FindProperty(nameof(EntitySettings.EntityTorso)), ArrayDrawStyle.Default, ". Torso Variant");

			UnityObjectField<EntityBodyPartConfiguration>(ref settings.EntityTail, ObjectNames.NicifyVariableName(nameof(EntitySettings.EntityTail)));
			DecreaseIndent();
		}

		protected virtual void DrawOtherSettings()
		{
			EntitySettings settings = target as EntitySettings;
			IncreaseIndent();
			if (!settings.EnableAI || (settings.AIConfiguration.EatBehavior != EatBehaviorType.DoesntEat))
			{
				settings.FoodEatDistance.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.FoodEatDistance)));
			}

			UnityObjectField<EntityMover>(ref settings.OverwritePrefab, ObjectNames.NicifyVariableName(nameof(EntitySettings.OverwritePrefab)));
			BoolField(ref settings.EnableAI, ObjectNames.NicifyVariableName(nameof(EntitySettings.EnableAI)));
			EditorGUI.BeginDisabledGroup(!settings.EnableAI);
			settings.AIConfiguration.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.AIConfiguration)),
										serializedObject.FindProperty(nameof(EntitySettings.AIConfiguration)));

			EditorGUI.EndDisabledGroup();
			DecreaseIndent();
		}
	}
}