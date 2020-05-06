using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
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
		Aggresive = 3,
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
		public DefaultableFloat IdleMoveSpeedMultiplier = new DefaultableFloat(null);
		public DefaultableFloat NextMovementDelay = new DefaultableFloat(null);

		public DefaultableFloat WanderingRadius = new DefaultableFloat(null);
		public Vector2[] PatrolPoints;

		//Active Behaviour
		public DefaultableFloat ViewRange = new DefaultableFloat(null);
		public DefaultableFloat ViewAngle = new DefaultableFloat(null);

		public DefaultableFloat ForgetTargetDistance = new DefaultableFloat(null);

		public DefaultableFloat AttackRange = new DefaultableFloat(null);
		public DefaultableInt AttackDamage = new DefaultableInt(null);
		public DefaultableFloat AttackCooldown = new DefaultableFloat(null);

		//Eating Behaviour
		public DefaultableInt MaxSizeIncrease = new DefaultableInt(null);
	}
}