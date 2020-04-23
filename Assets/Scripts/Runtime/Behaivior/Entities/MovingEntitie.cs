using Spectral.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaiviors
{
	public class MovingEntitie : SpectralMonoBehavior
	{
		protected const float INTENDED_ACCELERATION_CUTOFF = 0.1f;

		private Rigidbody bodyHead = default;
		[SerializeField] protected MovingEntitieSettings entitieSettings = default;

		protected GameObject head;
		protected List<GameObject> torsoParts;
		protected GameObject tail;

		protected Vector3 currentVelocity;

		protected Vector3? IntendedMoveDirection
		{
			get => intendedMoveDirection;
			set
			{
				if (value.HasValue && (intendedMoveDirection != value))
				{
					intendedMoveAngle = Mathf.Atan2(value.Value.x, value.Value.z) * Mathf.Rad2Deg;
				}
				intendedMoveDirection = value;
			}
		}
		protected float CurrentAcceleration { get; private set; }
		protected float intendedAcceleration;

		private float intendedMoveAngle;
		private Vector3? intendedMoveDirection;
		protected virtual void Start()
		{
			EntitieFactory.BuildEntitie(this, entitieSettings, entitieSettings.MinParts, Random.Range(0, 360));
		}
		protected virtual void Update()
		{
			UpdateIntendedMove();
			UpdateFacingDirection();
		}
		public void SetEntitieBodyInfo(GameObject head, List<GameObject> torsoParts, GameObject tail)
		{
			bodyHead = head.GetComponent<Rigidbody>();

			this.head = head;
			this.torsoParts = torsoParts;
			this.tail = tail;
		}
		private void UpdateIntendedMove()
		{
			if (!intendedMoveDirection.HasValue)
			{
				intendedAcceleration = 0;
			}
		}

		private void UpdateFacingDirection()
		{
			if(intendedAcceleration <= 0)
			{
				return;
			}

			float curMoveAngle = head.transform.eulerAngles.y;

			float newMoveAngle = Mathf.Abs(Mathf.DeltaAngle(curMoveAngle, intendedMoveAngle)) > entitieSettings.TurnSmoothAngle
								? Mathf.MoveTowardsAngle(curMoveAngle, intendedMoveAngle, entitieSettings.TurnSpeed * Time.deltaTime)
								: Mathf.LerpAngle(curMoveAngle, intendedMoveAngle, entitieSettings.TurnSpeed * Time.deltaTime * entitieSettings.TurnSmoothMultiplier);

			head.transform.eulerAngles = new Vector3(0, newMoveAngle, 0);
		}
		protected virtual void FixedUpdate()
		{
			SetMoveVelocity();
		}
		private void SetMoveVelocity()
		{
			float currentMoveSpeed = currentVelocity.magnitude;
			Vector3 currentMoveDirection = currentMoveSpeed > 0 ? (currentVelocity / currentMoveSpeed) : head.transform.forward;

			UpdateAccelerationValue(currentMoveDirection, currentMoveSpeed);
			float intendedMoveSpeed = CurrentAcceleration * entitieSettings.MoveSpeed;
			Vector3 intendedVelocity = head.transform.forward * intendedMoveSpeed;

			currentVelocity = Vector3.MoveTowards(currentVelocity, intendedVelocity, intendedMoveSpeed * Time.fixedDeltaTime);
			head.transform.position += currentVelocity * Time.fixedDeltaTime;
		}
		private void UpdateAccelerationValue(Vector3 currentMoveDirection, float currentMoveSpeed)
		{
			if (intendedAcceleration > INTENDED_ACCELERATION_CUTOFF)
			{
				float directionAgreement = Vector3.Dot(currentMoveDirection, head.transform.forward);
				float accelerationArea = entitieSettings.AccelerationAngle / 180;

				directionAgreement = currentMoveSpeed > 0 ? Mathf.Min(directionAgreement + (entitieSettings.MoveSpeed / currentMoveSpeed), 1) : 1;
				directionAgreement -= (1 - accelerationArea);
				directionAgreement = (directionAgreement > 0) ? (directionAgreement / accelerationArea) : (directionAgreement / (1 + accelerationArea));

				ChangeAcceleration(directionAgreement);
			}
			else if (CurrentAcceleration > 0)
			{
				ChangeAcceleration(-1);
			}
			else
			{
				//Apply velocity damp
				currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, GameManager.CurrentGameSettings.EntitieVelocityDamping * Time.fixedDeltaTime);
			}
		}
		private void ChangeAcceleration(float changeMultiplier)
		{
			float accelerationChange = changeMultiplier > 0 ? entitieSettings.Acceleration : entitieSettings.Deceleration;
			accelerationChange *= changeMultiplier * Time.fixedDeltaTime;

			float newAcceleration = Mathf.Clamp(CurrentAcceleration + accelerationChange, 0, intendedAcceleration);

			//If the intendedAcceleration clamp decelerated faster then would be allowed by the movement settings we will only decelerate by the settings amount
			float maxDeceleration = Time.fixedDeltaTime * entitieSettings.Deceleration;
			CurrentAcceleration = (CurrentAcceleration - newAcceleration) > maxDeceleration
									? Mathf.Clamp01(CurrentAcceleration - maxDeceleration)
									: newAcceleration;
		}
	}
}