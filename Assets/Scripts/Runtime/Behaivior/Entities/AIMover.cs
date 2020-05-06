using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaviours.Entities
{
	public class AIMover : EntityMover
	{
		#region Fields
		private const float REACHED_POINT_THRESHOLD = 1;
		private const float REACHED_POINT_THRESHOLD_SQR = REACHED_POINT_THRESHOLD * REACHED_POINT_THRESHOLD;
		private const float FLEE_FORESIGHT_DISTANCE = 15;

		private bool idle;

		private Vector2 currentTargetPosition;
		private Vector2 originSpawnPosition;
		private int patrolPointIndex = -1;

		private float attackCooldown;
		private float idleDecisionCooldown;

		private float viewAngleCos;

		private int eatenFoodObjectCount;
		#endregion
		#region Basics / Monobehaviour
		protected override void Start()
		{
			base.Start();
			ResetEnemy();
		}
		private void ResetEnemy()
		{
			SetIdleState(true);
			originSpawnPosition = Head.transform.position.XYZtoXZ();

			SetupViewAngles();
			ResetDecisionDelay();
		}
		private void SetupViewAngles()
		{
			float standardViewAngle = Mathf.Min(entitySettings.AIConfiguration.ViewAngle.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIViewAngle), 360) * Mathf.Deg2Rad;
			viewAngleCos = Mathf.Cos(standardViewAngle);
		}
		protected override void Update()
		{
			base.Update();
			AIBehaviour();
		}
		private void AIBehaviour()
		{
			if(attackCooldown > 0)
			{
				attackCooldown -= Time.deltaTime;
			}

			if (idle)
			{
				IdleBehaviour();
			}
			else
			{
				ActiveBehaviour();
			}
		}
		#endregion
		#region Idle
		private void IdleBehaviour()
		{
			if ((entitySettings.AIConfiguration.ActiveBehaviour != ActiveBehaviourType.Passive) && CanSeeTarget())
			{
				SetIdleState(false);
				return;
			}

			if (!CheckForFood() && (entitySettings.AIConfiguration.IdleBehaviour != IdleBehaviourType.Stay))
			{
				IdleMovement();
			}
		}
		private bool CheckForFood()
		{
			switch (entitySettings.AIConfiguration.EatBehavior)
			{
				default:
				case EatBehaviorType.DoesntEat:
					return false;
				case EatBehaviorType.EatsTillFull:
					if (eatenFoodObjectCount >= entitySettings.AIConfiguration.MaxSizeIncrease.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIMaxSizeIncrease))
					{
						return false;
					}
					else
					{
						break;
					}
				case EatBehaviorType.AlwaysEats:
					break;
			}

			FoodObject targetFood = FoodObject.GetNearestFoodObject(Head.transform.position.XYZtoXZ());
			if (targetFood)
			{
				Vector2 foodPos = targetFood.transform.position.XYZtoXZ();
				if (CanSeePoint(foodPos))
				{
					currentTargetPosition = foodPos;
					if (TryReachTargetPoint())
					{
						targetFood.Eat(this);
						ResetDecisionDelay();
					}

					return true;
				}
			}

			return false;
		}
		private void IdleMovement()
		{
			if (idleDecisionCooldown > 0)
			{
				idleDecisionCooldown -= Time.deltaTime;
				if (idleDecisionCooldown <= 0)
				{
					if (entitySettings.AIConfiguration.IdleBehaviour == IdleBehaviourType.Patrol)
					{
						GetNewPatrolPoint();
					}
					else
					{
						GetNewWanderPosition();
					}
				}
				else
				{
					return;
				}
			}

			if (TryReachTargetPoint())
			{
				ResetDecisionDelay();
			}
		}
		private void GetNewWanderPosition()
		{
			idleDecisionCooldown = -1;
			float directionAngle = Random.Range(0f, 360f);
			Vector3 newWanderPosition = Head.transform.position 
				+ (new Vector3(Mathf.Sin(directionAngle * Mathf.Deg2Rad), 0, Mathf.Cos(directionAngle * Mathf.Deg2Rad)) 
				* entitySettings.AIConfiguration.WanderingRadius.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIWanderingRadius) 
				* Random.Range(0.25f, 1));

			currentTargetPosition = newWanderPosition.XYZtoXZ().ClampIntoLevelBounds();
		}
		private void GetNewPatrolPoint()
		{
			idleDecisionCooldown = -1;
			patrolPointIndex = (patrolPointIndex + 1) % entitySettings.AIConfiguration.PatrolPoints.Length;
			currentTargetPosition = (originSpawnPosition + entitySettings.AIConfiguration.PatrolPoints[patrolPointIndex]).ClampIntoLevelBounds();
		}
		private void ResetDecisionDelay()
		{
			idleDecisionCooldown = Mathf.Max(0.001f, (entitySettings.AIConfiguration.NextMovementDelay.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AINextMovementDelay) * Random.Range(0.75f, 1.25f)));
		}
		#endregion
		#region Active
		private void ActiveBehaviour()
		{
			if (IsTargetOutOfInterestRange())
			{
				SetIdleState(true);
				return;
			}

			if (entitySettings.AIConfiguration.ActiveBehaviour == ActiveBehaviourType.Aggresive)
			{
				ChaseTarget();
			}
			else
			{
				FleeFromTarget();
			}
		}
		private void ChaseTarget()
		{
			Transform target = GetCurrentTarget();
			if (!target)
			{
				SetIdleState(true);
				return;
			}

			currentTargetPosition = target.position.XYZtoXZ();
			if (TryReachTargetPoint(entitySettings.AIConfiguration.AttackRange.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIAttackRange)))
			{
				attackCooldown = entitySettings.AIConfiguration.AttackCooldown.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIAttackCooldown);
				SetIdleState(true);
				PlayerMover.Instance.Damage(entitySettings.AIConfiguration.AttackDamage.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIAttackDamage));
				OnEat();
			}
		}
		private void FleeFromTarget()
		{
			//Find point where to flee to
			Vector2 fleeDirection = (Head.transform.position - PlayerMover.Instance.Head.transform.position).XYZtoXZ().normalized;
			Vector2 rawFleePoint = Head.transform.position.XYZtoXZ() + (fleeDirection * FLEE_FORESIGHT_DISTANCE);

			Vector2 clampedFleePoint = rawFleePoint.ClampIntoLevelBounds();
			Vector2 clampDif = clampedFleePoint - rawFleePoint;
			clampedFleePoint += (Mathf.Abs(clampDif.x) > Mathf.Abs(clampDif.y)) ? new Vector2(clampDif.x, clampDif.y / 2) : new Vector2(clampDif.x / 2, clampDif.y);

			currentTargetPosition = clampedFleePoint.ClampIntoLevelBounds();
			TryReachTargetPoint();
		}
		#endregion
		#region Utilities
		private Transform GetCurrentTarget()
		{
			if (!PlayerMover.Existant)
			{
				return null;
			}

			Vector2 curForward = Head.transform.forward.XYZtoXZ();
			float shortestDistance = GetVisionOffset(PlayerMover.Instance.Head.transform);
			int torsoIndex = -1;

			for (int i = 0; i < GameManager.CurrentGameSettings.AttackablePlayerTorsoCount; i++)
			{
				if (i >= PlayerMover.Instance.TorsoParts.Count)
				{
					break;
				}

				float distance = GetVisionOffset(PlayerMover.Instance.TorsoParts[i].transform);
				if(distance < shortestDistance)
				{
					shortestDistance = distance;
					torsoIndex = i;
				}
			}

			return torsoIndex >= 0 ? PlayerMover.Instance.TorsoParts[torsoIndex].transform : PlayerMover.Instance.Head.transform;
			float GetVisionOffset(Transform target)
			{
				Vector2 dir = (target.position - Head.transform.position).XYZtoXZ().normalized;
				float dot = Vector2.Dot(dir, curForward);
				return 2 - (dot + 1);
			}
		}
		private bool TryReachTargetPoint(float stopDistance = REACHED_POINT_THRESHOLD_SQR)
		{
			Vector2 diff = currentTargetPosition - Head.transform.position.XYZtoXZ();
			float dist = diff.sqrMagnitude;

			if (dist < stopDistance)
			{
				intendedAcceleration = 0;
				return true;
			}
			else
			{
				dist = Mathf.Sqrt(dist);
				IntendedMoveDirection = (diff / dist).XZtoXYZ();

				float maxPossibleAcceleration = idle ? entitySettings.AIConfiguration.IdleMoveSpeedMultiplier.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIIdleMoveSpeedMultiplier) : 1;
				float wantedAcceleration = dist * entitySettings.MoveSpeed.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityMoveSpeed);

				intendedAcceleration = Mathf.Min(wantedAcceleration, maxPossibleAcceleration);
				return false;
			}
		}
		private bool IsTargetOutOfInterestRange()
		{
			if (!PlayerMover.Existant)
			{
				return true;
			}

			float dist = (Head.transform.position - PlayerMover.Instance.Head.transform.position).XYZtoXZ().sqrMagnitude;
			float maxDist = entitySettings.AIConfiguration.ForgetTargetDistance.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIForgetTargetDistance);

			return dist > maxDist * maxDist;
		}
		private bool CanSeeTarget()
		{
			if (!PlayerMover.Existant || (attackCooldown > 0))
			{
				return false;
			}

			return CanSeePoint(PlayerMover.Instance.Head.transform.position.XYZtoXZ()) || (PlayerMover.Instance.Tail && CanSeePoint(PlayerMover.Instance.Tail.transform.position.XYZtoXZ()));
		}
		private bool CanSeePoint(Vector2 point)
		{
			Vector2 diff = point - Head.transform.position.XYZtoXZ();
			float dist = diff.sqrMagnitude;
			float viewRange = entitySettings.AIConfiguration.ViewRange.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIViewRange);

			if (dist > (viewRange * viewRange))
			{
				return false;
			}

			float dot = Vector2.Dot(Head.transform.forward.XYZtoXZ(), diff / Mathf.Sqrt(dist));
			return dot > viewAngleCos;
		}
		private void SetIdleState(bool state)
		{
			idle = state;
			IntendedMoveDirection = null;
		}
		#endregion
		#region Overrides
		public override void OnEat(FoodObject target = null)
		{
			if(eatenFoodObjectCount < entitySettings.AIConfiguration.MaxSizeIncrease.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIMaxSizeIncrease))
			{
				base.OnEat(target);
			}
			eatenFoodObjectCount++;
		}
		#endregion
		#region Gizmos
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			Gizmos.color = Color.red;
			Gizmos.DrawLine(Head.transform.position, currentTargetPosition.XZtoXYZ());

			if(entitySettings.AIConfiguration.IdleBehaviour == IdleBehaviourType.Wander)
			{
				UnityEditor.Handles.color = Color.cyan / 3;
				UnityEditor.Handles.DrawSolidDisc(Head.transform.position, Vector3.up, entitySettings.AIConfiguration.WanderingRadius.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIWanderingRadius));
			}
			else if(entitySettings.AIConfiguration.IdleBehaviour == IdleBehaviourType.Patrol)
			{
				if(patrolPointIndex == -1)
				{
					return;
				}
				for(int i = patrolPointIndex; i < (patrolPointIndex + entitySettings.AIConfiguration.PatrolPoints.Length); i++)
				{
					int index = i % entitySettings.AIConfiguration.PatrolPoints.Length;
					int lastIndex = (index - 1 + entitySettings.AIConfiguration.PatrolPoints.Length) % entitySettings.AIConfiguration.PatrolPoints.Length;

					Gizmos.color = (Color.HSVToRGB(index / (float)entitySettings.AIConfiguration.PatrolPoints.Length, 1, 1) + Color.white) / 3;
					Gizmos.DrawLine((originSpawnPosition + entitySettings.AIConfiguration.PatrolPoints[lastIndex]).XZtoXYZ(), (originSpawnPosition + entitySettings.AIConfiguration.PatrolPoints[index]).XZtoXYZ());
					Gizmos.DrawSphere((originSpawnPosition + entitySettings.AIConfiguration.PatrolPoints[index]).XZtoXYZ(), REACHED_POINT_THRESHOLD);
				}
			}

			UnityEditor.Handles.color = (Color.green + Color.red) / 3;
			float viewDistance = entitySettings.AIConfiguration.ViewRange.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIViewRange);
			float standardViewAngle = Mathf.Min(entitySettings.AIConfiguration.ViewAngle.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.AIViewAngle), 360);

			float leftAngle = Head.transform.eulerAngles.y - standardViewAngle;
			Vector3 leftForward = new Vector3(Mathf.Sin(leftAngle * Mathf.Deg2Rad), 0, Mathf.Cos(leftAngle * Mathf.Deg2Rad));

			UnityEditor.Handles.DrawSolidArc(Head.transform.position, Vector3.up, leftForward, standardViewAngle * 2, viewDistance);
		}
#endif
	#endregion
	}
}