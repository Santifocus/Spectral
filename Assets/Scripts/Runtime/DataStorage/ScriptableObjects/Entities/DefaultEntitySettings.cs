using UnityEngine;

namespace Spectral.Runtime.DataStorage
{
	public class DefaultEntitySettings : SpectralScriptableObject
	{
		[Header("Speed")] public float EntityMoveSpeed = 15;
		public float EntityVelocityDamping = 0.5f;

		[Space(6)] [Header("Acceleration")] public float EntityAcceleration = 7;
		public float EntityDeceleration = 20;
		public float EntityAccelerationAngle = 60;

		[Space(6)] [Header("Turning")] public float EntityTurnSpeed = 180;
		public float EntityTurnAcceleration = 0.5f;
		public float EntityTurnSmoothAngle = 40;
		public float EntityTurnSmoothMultiplier = 0.025f;

		[Space(6)] [Header("Body Settings")] public float EntityPartMinimumScale = 0.8f;
		public float EntityScaleChangePerPart = 0.05f;

		[Space(6)] [Header("Other Settings")] public float EntityEatDistance = 1;

		[Space(6)] [Header("AI Settings")]

		//Idle Behaviour
		public float AIIdleMoveSpeedMultiplier = 0.3f;

		public float AINextMovementDelay = 3;

		public float AIWanderingRadius = 5;

		//Active Behaviour
		public float AIViewRange = 10;
		public float AIViewAngle = 50;

		public float AIForgetTargetDistance = 20;

		public float AIAttackRange = 2;
		public int AIAttackDamage = 1;
		public int AIAttackForceImpact = 0;
		public float AIAttackCooldown = 3;

		//Eating Behaviour
		public int AIMaxSizeIncrease = 3;
	}
}