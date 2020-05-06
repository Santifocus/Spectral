using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Spectral.EditorInspector.EditorUtils;

namespace Spectral.EditorInspector
{
	public static class EntityAIConfigurationDrawer
	{
		public static void Draw(this EntityAIConfiguration self, string label, SerializedProperty property)
		{
			Foldout(label, property);

			if (property.isExpanded)
			{
				EditorGUILayout.HelpBox(GetInfoAboutBehaviour(self), MessageType.Info);

				LineBreak();

				EnumField<IdleBehaviourType>(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.IdleBehaviour)), ref self.IdleBehaviour);
				EnumField<ActiveBehaviourType>(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.ActiveBehaviour)), ref self.ActiveBehaviour);
				EnumField<EatBehaviorType>(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.EatBehavior)), ref self.EatBehavior);

				if (self.IdleBehaviour != IdleBehaviourType.Stay 
					|| self.ActiveBehaviour != ActiveBehaviourType.Passive 
					|| self.EatBehavior != EatBehaviorType.DoesntEat)
				{
					LineBreak();
				}

				if (self.IdleBehaviour != IdleBehaviourType.Stay)
				{
					Header("Idle Settings");

					self.IdleMoveSpeedMultiplier.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.IdleMoveSpeedMultiplier)));
					self.NextMovementDelay.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.NextMovementDelay)));

					if(self.IdleBehaviour == IdleBehaviourType.Wander)
					{
						self.WanderingRadius.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.WanderingRadius)));
					}
					else
					{
						DrawArray<Vector2>(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.PatrolPoints)), ref self.PatrolPoints, property.FindPropertyRelative(nameof(EntityAIConfiguration.PatrolPoints)), DrawVector2, ". Point");
					}

					if (self.ActiveBehaviour != ActiveBehaviourType.Passive || self.EatBehavior != EatBehaviorType.DoesntEat)
					{
						LineBreak();
					}
				}

				if(self.ActiveBehaviour != ActiveBehaviourType.Passive)
				{
					Header("Active Settings");

					self.ViewRange.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.ViewRange)));
					self.ViewAngle.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.ViewAngle)));
					
					self.ForgetTargetDistance.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.ForgetTargetDistance)));

					if(self.ActiveBehaviour == ActiveBehaviourType.Aggresive)
					{
						GUILayout.Space(8);
						self.AttackRange.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.AttackRange)));
						self.AttackDamage.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.AttackDamage)));
						self.AttackCooldown.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.AttackCooldown)));
					}

					if (self.EatBehavior != EatBehaviorType.DoesntEat)
					{
						LineBreak();
					}
				}

				if(self.EatBehavior != EatBehaviorType.DoesntEat)
				{
					Header("Eat Settings");
					self.MaxSizeIncrease.Draw(ObjectNames.NicifyVariableName(nameof(EntityAIConfiguration.MaxSizeIncrease)));
				}
			}
		}
		private static Vector2 DrawVector2(string label, Vector2 self, SerializedProperty property)
		{
			Vector2Field(label, ref self);
			return self;
		}
		private static string GetInfoAboutBehaviour(EntityAIConfiguration self)
		{
			StringBuilder info = new StringBuilder();
			info.Append("Behaves as follows:");

			switch (self.IdleBehaviour)
			{
				case IdleBehaviourType.Stay:
					info.Append("\nDoesnt move around when idle.");
					break;
				case IdleBehaviourType.Wander:
					info.Append("\nMoves into a random direction and waits there for a set amount of time.");
					break;
				case IdleBehaviourType.Patrol:
					info.Append("\nMoves in a defined pattern which loops when completed.");
					break;
			}

			switch (self.ActiveBehaviour)
			{
				case ActiveBehaviourType.Passive:
					info.Append("\nDoesnt react to any targets.");
					break;
				case ActiveBehaviourType.Shy:
					info.Append("\nWhen it spots a target it will try to run away until it is a set distance to the target.");
					break;
				case ActiveBehaviourType.Aggresive:
					info.Append("\nWhen it spots a target it will try to attack it but forgets about it when it moved further then the set distance.");
					break;
			}

			switch (self.EatBehavior)
			{
				case EatBehaviorType.DoesntEat:
					info.Append("\nDoesnt look for food nor eats any.");
					break;
				case EatBehaviorType.EatsTillFull:
					info.Append("\nIt will eat any food it spots until it reached the set max size increase.");
					break;
				case EatBehaviorType.AlwaysEats:
					info.Append("\nIt will eat any food it spots, however if the set max size increase is reached it will not grow any further.");
					break;
			}

			if(self.IdleBehaviour == IdleBehaviourType.Stay && self.ActiveBehaviour == ActiveBehaviourType.Passive && self.EatBehavior == EatBehaviorType.DoesntEat)
			{
				info.Append("\n\nThis Entity will never move on its own.");
			}
			return info.ToString();
		}
	}
}