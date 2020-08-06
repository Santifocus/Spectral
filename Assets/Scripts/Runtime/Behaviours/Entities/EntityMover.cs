using System.Collections.Generic;
using Spectral.Runtime.DataStorage;
using Spectral.Runtime.Factories;
using Spectral.Runtime.FX.Handling;
using UnityEngine;

namespace Spectral.Runtime.Behaviours.Entities
{
	public class EntityMover : LevelPlaneBehavior
	{
		[SerializeField] private EntitySettings entitySettings = default;
		[SerializeField] private int spawnTotalBodySize = 1;

		protected bool Alive;
		public EntitySettings EntitySettings => entitySettings;
		public EntityBodyPart Head { get; set; }
		public List<EntityBodyPart> TorsoParts { get; set; }
		public EntityBodyPart Tail { get; set; }

		public Vector3 CurrentVelocity { get; private set; }

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
		private bool initialised;

		protected virtual void Start()
		{
			if (!initialised)
			{
				ForceInitialise();
			}

			Alive = true;
		}

		private void ForceInitialise()
		{
			if (entitySettings)
			{
				if (spawnTotalBodySize >= entitySettings.MinParts)
				{
					EntityFactory.BuildEntityBody(this, entitySettings, spawnTotalBodySize, Random.Range(0, 360));
				}
				else
				{
#if SPECTRAL_DEBUG
					gameObject.SetActive(false);
					throw new System.Exception("A scene injected Entity (" + name +
												") had a total body size which was below the minimum count of body parts allowed by the Settings... Disabling");
#else
					Destroy(gameObject);
#endif
				}
			}
			else
			{
#if SPECTRAL_DEBUG
				gameObject.SetActive(false);
				throw new System.NullReferenceException("A scene injected Entity (" + name + ") had no Settings applied and could therefore not be created... Disabling");
#else
				Destroy(gameObject);
#endif
			}
		}

		protected virtual void Update()
		{
			UpdateFacingDirection();
		}

		public void Initialise(EntitySettings entitySettings)
		{
			initialised = true;
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
			//Check for current state of the velocity
			Vector3 forward = Head.transform.forward;
			float currentMoveSpeed = CurrentVelocity.magnitude;
			Vector3 currentMoveDirection = currentMoveSpeed > 0 ? CurrentVelocity / currentMoveSpeed : forward;

			//Update the acceleration based on the current state
			UpdateAcceleration(currentMoveDirection, forward, currentMoveSpeed);

			//Lerp toward the intended movement based on the current
			float targetMoveSpeed = CurrentAcceleration * entitySettings.MoveSpeed;
			Vector3 targetVelocity = forward            * targetMoveSpeed;
			CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, targetVelocity, targetMoveSpeed * Time.fixedDeltaTime);

			//Update the position based on the speed
			Head.transform.position += CurrentVelocity * Time.fixedDeltaTime;
		}

		private void UpdateAcceleration(Vector3 currentMoveDirection, Vector3 forward, float currentMoveSpeed)
		{
			if (IntendedAcceleration > 0)
			{
				float directionAgreement = Vector3.Dot(currentMoveDirection, forward);
				float accelerationArea = entitySettings.AccelerationAngle / 180;
				directionAgreement = currentMoveSpeed > 0
										? Mathf.Min(directionAgreement + (entitySettings.MoveSpeed / currentMoveSpeed), 1)
										: 1;

				directionAgreement -= 1 - accelerationArea;
				directionAgreement = directionAgreement > 0 ? directionAgreement / accelerationArea : directionAgreement / (1 + accelerationArea);
				ChangeAcceleration(directionAgreement);
			}
			else if (CurrentAcceleration > 0)
			{
				ChangeAcceleration(-1);
			}

			//Apply velocity damp
			if (CurrentAcceleration < 0.00001f)
			{
				CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, entitySettings.VelocityDamping * Time.fixedDeltaTime);
			}
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

		public virtual void OnEat(FoodObject target = null)
		{
			FXInstanceUtils.ExecuteFX(entitySettings.EatFX, transform);
			EntityFactory.IncreaseEntitySize(this, entitySettings);
			if (target != null)
			{
				target.Eat();
			}

			//Play effects on parts
			int bodySize = EntityFactory.GetEntitySize(this);
			for (int i = 0; i < bodySize; i++)
			{
				EntityFactory.GetBodyPartFromIndex(this, i).FeedbackPlayer.PlayEatEffect();
			}
		}

		public virtual void Damage(int amount = 1, bool silent = false)
		{
			for (int i = 0; i < amount; i++)
			{
				if (!Alive)
				{
					break;
				}

				EntityFactory.DecreaseEntitySize(this, entitySettings);
			}

			if (!silent)
			{
				FXInstanceUtils.ExecuteFX(entitySettings.DamageFX, transform);

				//Play effects on parts
				int bodySize = EntityFactory.GetEntitySize(this);
				for (int i = 0; i < bodySize; i++)
				{
					EntityFactory.GetBodyPartFromIndex(this, i).FeedbackPlayer.PlayDamageEffect();
				}
			}
		}

		public virtual void Death()
		{
			Alive = false;
			EntityFactory.TearDownEntity(this);
		}
	}
}