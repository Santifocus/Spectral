using System.Collections.Generic;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Behaviours.Entities
{
	public class EntityMover : SpectralMonoBehavior
	{
		[SerializeField] protected EntitySettings entitySettings = default;
		[SerializeField] private int spawnTotalBodySize = 1;

		protected bool Alive;
		public EntityBodyPart Head { get; set; }
		public List<EntityBodyPart> TorsoParts { get; set; }
		public EntityBodyPart Tail { get; set; }

		protected Vector3 CurrentVelocity;

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
		protected float IntendedAcceleration;

		protected float EatDistance => entitySettings.FoodEatDistance * Head.transform.localScale.x;

		private float intendedMoveAngle;
		private Vector3? intendedMoveDirection;
		private bool isSetup;

		protected virtual void Start()
		{
			if (!isSetup)
			{
				ForceSpawnSetup();
			}

			Alive = true;
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
					throw new System.Exception("A scene injected Entity (" + name +
												") had a total body size which was below the minimum count of body parts allowed by the Settings... Disabling");
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
		}

		private void UpdateFacingDirection()
		{
			if (IntendedAcceleration <= 0)
			{
				return;
			}

			float curMoveAngle = Head.transform.eulerAngles.y;
			float turnAmount = entitySettings.TurnSpeed;
			turnAmount *= Time.deltaTime;
			float newMoveAngle = Mathf.Abs(Mathf.DeltaAngle(curMoveAngle, intendedMoveAngle)) >
								entitySettings.TurnSmoothAngle
									? Mathf.MoveTowardsAngle(curMoveAngle, intendedMoveAngle, turnAmount)
									: Mathf.LerpAngle(curMoveAngle, intendedMoveAngle, turnAmount * entitySettings.TurnSmoothMultiplier);

			Head.transform.eulerAngles = new Vector3(0, newMoveAngle, 0);
		}

		private void UpdateModelSideTurn() { }

		protected virtual void FixedUpdate()
		{
			SetMoveVelocity();
		}

		public void AddForceImpact(Vector2 force)
		{
			CurrentVelocity += force.XZtoXYZ();
		}

		private void SetMoveVelocity()
		{
			float currentMoveSpeed = CurrentVelocity.magnitude;
			Vector3 currentMoveDirection = currentMoveSpeed > 0 ? CurrentVelocity / currentMoveSpeed : Head.transform.forward;
			UpdateAccelerationValue(currentMoveDirection, currentMoveSpeed);
			float intendedMoveSpeed = CurrentAcceleration     * entitySettings.MoveSpeed;
			Vector3 intendedVelocity = Head.transform.forward * intendedMoveSpeed;
			CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, intendedVelocity, intendedMoveSpeed * Time.fixedDeltaTime);
			Head.transform.position += CurrentVelocity * Time.fixedDeltaTime;
		}

		private void UpdateAccelerationValue(Vector3 currentMoveDirection, float currentMoveSpeed)
		{
			if (IntendedAcceleration > 0)
			{
				float directionAgreement = Vector3.Dot(currentMoveDirection, Head.transform.forward);
				float accelerationArea = entitySettings.AccelerationAngle / 180;
				directionAgreement = currentMoveSpeed > 0
										? Mathf
											.Min(directionAgreement + (entitySettings.MoveSpeed / currentMoveSpeed),
												1)
										: 1;

				directionAgreement -= 1 - accelerationArea;
				directionAgreement = directionAgreement > 0 ? directionAgreement / accelerationArea : directionAgreement / (1 + accelerationArea);
				ChangeAcceleration(directionAgreement);
			}
			else if (CurrentAcceleration > 0)
			{
				ChangeAcceleration(-1);
			}
			else
			{
				//Apply velocity damp
				CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, entitySettings.VelocityDamping * Time.fixedDeltaTime);
			}
		}

		public virtual void OnEat(FoodObject target = null)
		{
			EntityFactory.IncreaseEntitySize(this, entitySettings);
		}

		public void Damage(int amount = 1)
		{
			for (int i = 0; i < amount; i++)
			{
				if (!Alive)
				{
					break;
				}

				EntityFactory.DecreaseEntitySize(this, entitySettings);
			}
		}

		public void Death()
		{
			Alive = false;
		}

		private void ChangeAcceleration(float changeMultiplier)
		{
			float accelerationChange = changeMultiplier > 0 ? entitySettings.Acceleration : entitySettings.Deceleration;
			accelerationChange *= changeMultiplier * Time.fixedDeltaTime;
			float newAcceleration = Mathf.Clamp(CurrentAcceleration + accelerationChange, 0, IntendedAcceleration);

			//If the intendedAcceleration clamp decelerated faster then would be allowed by the movement settings we will only decelerate by the settings amount
			float maxDeceleration = Time.fixedDeltaTime * entitySettings.Deceleration;
			CurrentAcceleration = (CurrentAcceleration - newAcceleration) > maxDeceleration
									? Mathf.Clamp01(CurrentAcceleration - maxDeceleration)
									: newAcceleration;
		}
	}
}