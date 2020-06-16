using UnityEngine;

namespace Spectral.Runtime.DataStorage
{
	public enum IdleBehaviourType
	{
		Stay = 1,
		Wander = 2,
		Patrol = 3,
	}

	public enum ActiveBehaviourType
	{
		Passive = 1,
		Shy = 2,
		Aggressive = 3,
	}

	public enum EatBehaviorType
	{
		DoesntEat = 1,
		EatsTillFull = 2,
		AlwaysEats = 3,
	}

	[System.Serializable]
	public class EntityAIConfiguration
	{
		public IdleBehaviourType IdleBehaviour = IdleBehaviourType.Wander;
		public ActiveBehaviourType ActiveBehaviour = ActiveBehaviourType.Shy;
		public EatBehaviorType EatBehavior = EatBehaviorType.DoesntEat;

		//Idle Behaviour
		public DefaultableFloat IdleMoveSpeedMultiplier = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIIdleMoveSpeedMultiplier);
		public DefaultableFloat NextMovementDelay = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AINextMovementDelay);

		public DefaultableFloat WanderingRadius = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIWanderingRadius);
		public Vector2[] PatrolPoints;

		//Active Behaviour
		public DefaultableFloat ViewRange = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIViewRange);
		public DefaultableFloat ViewAngle = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIViewAngle);

		public DefaultableFloat ForgetTargetDistance = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIForgetTargetDistance);

		public DefaultableFloat AttackRange = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIAttackRange);
		public DefaultableInt AttackDamage = new DefaultableInt(() => GameSettings.Current.DefaultEntitySettings.AIAttackDamage);
		public DefaultableFloat AttackForceImpact = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIAttackForceImpact);
		public DefaultableFloat AttackCooldown = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.AIAttackCooldown);

		//Eating Behaviour
		public DefaultableInt MaxSizeIncrease = new DefaultableInt(() => GameSettings.Current.DefaultEntitySettings.AIMaxSizeIncrease);
	}
}