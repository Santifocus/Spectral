using Spectral.Behaviours.Entities;
using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Spectral.EditorInspector.EditorUtils;

namespace Spectral.EditorInspector
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
			DrawInFoldoutHeader("Movement Settings", ref movementSettingsOpen, DrawMovementSettings);
			DrawInFoldoutHeader("Body Settings", ref bodySettingsOpen, DrawBodySettings);
			DrawInFoldoutHeader("Other Settings", ref otherSettingsOpen, DrawOtherSettings);
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
			IntField(ObjectNames.NicifyVariableName(nameof(EntitySettings.MinParts)), ref settings.MinParts);

			settings.PartMinimumScale.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.PartMinimumScale)));
			settings.ScaleChangePerPart.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.ScaleChangePerPart)));

			UnityObjectField<EntityBodyPartSettings>(ObjectNames.NicifyVariableName(nameof(EntitySettings.EntityHead)), ref settings.EntityHead);
			EnumField<ChooseStyle>(ObjectNames.NicifyVariableName(nameof(EntitySettings.TorsoChooseStyle)), ref settings.TorsoChooseStyle);
			DrawUnityObjectArray<EntityBodyPartSettings>(ObjectNames.NicifyVariableName(nameof(EntitySettings.EntityTorso)), ref settings.EntityTorso, serializedObject.FindProperty(nameof(EntitySettings.EntityTorso)),". Torso Variant");
			UnityObjectField<EntityBodyPartSettings>(ObjectNames.NicifyVariableName(nameof(EntitySettings.EntityTail)), ref settings.EntityTail);
			DecreaseIndent();
		}
		protected virtual void DrawOtherSettings()
		{
			EntitySettings settings = target as EntitySettings;
			IncreaseIndent();

			UnityObjectField<EntityMover>(ObjectNames.NicifyVariableName(nameof(EntitySettings.OverwritePrefab)), ref settings.OverwritePrefab);

			BoolField(ObjectNames.NicifyVariableName(nameof(EntitySettings.EnableAI)), ref settings.EnableAI);
			EditorGUI.BeginDisabledGroup(!settings.EnableAI);
			settings.AIConfiguration.Draw(ObjectNames.NicifyVariableName(nameof(EntitySettings.AIConfiguration)), serializedObject.FindProperty(nameof(EntitySettings.AIConfiguration)));
			EditorGUI.EndDisabledGroup();

			DecreaseIndent();
		}
	}
}