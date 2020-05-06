using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaviours.Entities
{
	public class EntityMover : SpectralMonoBehavior
	{
		private Rigidbody velocityCore = default;
		[SerializeField] protected EntitySettings entitySettings = default;
		[SerializeField] private int spawnTotalBodySize = 1;

		protected bool alive;
		public EntityBodyPart Head { get; set; }
		public List<EntityBodyPart> TorsoParts { get; set; }
		public EntityBodyPart Tail { get; set; }

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

		protected float EatDistance => entitySettings.FoodEatDistance.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityEatDistance) * Head.transform.localScale.x;

		private float intendedMoveAngle;
		private Vector3? intendedMoveDirection;
		private bool isSetup;
		protected virtual void Start()
		{
			if (!isSetup)
			{
				ForceSpawnSetup();
			}
			alive = true;
		}
		private void ForceSpawnSetup()
		{
			if (entitySettings)
			{
				if (spawnTotalBodySize >= entitySettings.MinParts)
				{
					EntityFactory.BuildEntityBody(this, entitySettings, spawnTotalBodySize, Random.Range(0, 360));
				}
				else
				{
					gameObject.SetActive(false);
#if SPECTRAL_DEBUG
					throw new System.Exception("A scene injected Entity (" + name + ") had a total body size which was below the minumum count of body parts allowed by the Settings... Disabling");
#endif
				}
			}
			else
			{
				gameObject.SetActive(false);
#if SPECTRAL_DEBUG
				throw new System.NullReferenceException("A scene injected Entity (" + name + ") had no Settings applied and could therefore not be created... Disabling");
#endif
			}
		}
		protected virtual void Update()
		{
			UpdateFacingDirection();
		}
		public void SetupEntity(EntitySettings entitySettings)
		{
			isSetup = true;
			this.entitySettings = entitySettings;
			velocityCore = Head.GetComponent<Rigidbody>();
		}
		private void UpdateFacingDirection()
		{
			b();
			return;
			if (intendedAcceleration <= 0)
			{
				return;
			}

			float curMoveAngle = Head.transform.eulerAngles.y;
			float turnAmount = entitySettings.TurnSpeed.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityTurnSpeed);
			turnAmount *= Time.deltaTime;

			float newMoveAngle = Mathf.Abs(Mathf.DeltaAngle(curMoveAngle, intendedMoveAngle)) > entitySettings.TurnSmoothAngle.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityTurnSmoothAngle)
								? Mathf.MoveTowardsAngle(curMoveAngle, intendedMoveAngle, turnAmount)
								: Mathf.LerpAngle(curMoveAngle, intendedMoveAngle, turnAmount * entitySettings.TurnSmoothMultiplier.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityTurnSmoothMultiplier));

			Head.transform.eulerAngles = new Vector3(0, newMoveAngle, 0);
		}
		private void b()
		{
			if (!intendedMoveDirection.HasValue)
			{
				return;
			}
			float turnAmount = entitySettings.TurnSpeed.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityTurnSpeed) / 180;
			turnAmount *= Time.deltaTime;

			Head.transform.forward = Vector3.Slerp(Head.transform.forward, intendedMoveDirection.Value, turnAmount);
		}
		protected virtual void FixedUpdate()
		{
			SetMoveVelocity();
		}
		private void SetMoveVelocity()
		{
			float currentMoveSpeed = currentVelocity.magnitude;
			Vector3 currentMoveDirection = currentMoveSpeed > 0 ? (currentVelocity / currentMoveSpeed) : Head.transform.forward;

			UpdateAccelerationValue(currentMoveDirection, currentMoveSpeed);
			float intendedMoveSpeed = CurrentAcceleration * entitySettings.MoveSpeed.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityMoveSpeed);
			Vector3 intendedVelocity = Head.transform.forward * intendedMoveSpeed;

			currentVelocity = Vector3.MoveTowards(currentVelocity, intendedVelocity, intendedMoveSpeed * Time.fixedDeltaTime);
			Head.transform.position += currentVelocity * Time.fixedDeltaTime;
		}
		private void UpdateAccelerationValue(Vector3 currentMoveDirection, float currentMoveSpeed)
		{
			if (intendedAcceleration > 0)
			{
				float directionAgreement = Vector3.Dot(currentMoveDirection, Head.transform.forward);
				float accelerationArea = entitySettings.AccelerationAngle.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityAccelerationAngle) / 180;

				directionAgreement = currentMoveSpeed > 0 ? Mathf.Min(directionAgreement + (entitySettings.MoveSpeed.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityMoveSpeed) / currentMoveSpeed), 1) : 1;
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
				currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, entitySettings.VelocityDamping.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityVelocityDamping) * Time.fixedDeltaTime);
			}
		}
		public virtual void OnEat(FoodObject target = null)
		{
			EntityFactory.IncreaseEntitySize(this, entitySettings);
		}
		public void Damage(int amount = 1)
		{
			for(int i = 0; i < amount; i++)
			{
				if (!alive)
				{
					break;
				}
				EntityFactory.DecreaseEntitySize(this, entitySettings);
			}
		}
		public void Death()
		{
			alive = false;
		}

		private void ChangeAcceleration(float changeMultiplier)
		{
			float accelerationChange = changeMultiplier > 0
										? entitySettings.Acceleration.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityAcceleration)
										: entitySettings.Deceleration.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityDeceleration);
			accelerationChange *= changeMultiplier * Time.fixedDeltaTime;

			float newAcceleration = Mathf.Clamp(CurrentAcceleration + accelerationChange, 0, intendedAcceleration);

			//If the intendedAcceleration clamp decelerated faster then would be allowed by the movement settings we will only decelerate by the settings amount
			float maxDeceleration = Time.fixedDeltaTime * entitySettings.Deceleration.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityDeceleration);
			CurrentAcceleration = (CurrentAcceleration - newAcceleration) > maxDeceleration
									? Mathf.Clamp01(CurrentAcceleration - maxDeceleration)
									: newAcceleration;
		}
	}
}