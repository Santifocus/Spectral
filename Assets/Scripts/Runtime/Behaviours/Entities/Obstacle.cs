using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.DataStorage;
using Spectral.Runtime.Factories;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class Obstacle : LevelPlaneBehavior
	{
		private const float DIRECTION_FLIP_CHECK_COOLDOWN = 1;
		[SerializeField] private ObstacleSettings obstacleSettings = default;
		private Vector2 movementDirection;
		private Vector2 directionFlipCooldown;

		private float damageCooldown;

		private void Start()
		{
			float moveAngle = Random.value * 360;
			movementDirection = new Vector2(Mathf.Cos(moveAngle * Mathf.Deg2Rad), Mathf.Sin(moveAngle * Mathf.Deg2Rad));
			Instantiate(obstacleSettings.Model, transform);
		}

		public void Initialise(ObstacleSettings obstacleSettings)
		{
			this.obstacleSettings = obstacleSettings;
		}

		private void Update()
		{
			if (damageCooldown > 0)
			{
				damageCooldown -= Time.deltaTime;
			}
		}

		private void FixedUpdate()
		{
			CheckForDirectionFlip();
			transform.position += movementDirection.XZtoXYZ() * (obstacleSettings.MoveSpeed * Time.fixedDeltaTime);
			CheckForContact();
		}

		private void CheckForDirectionFlip()
		{
			if (!PlaneLevelIndex.HasValue)
			{
				return;
			}

			Vector2 normalizedPositionInLevel = transform.position.XYZtoXZ().NormalizeFromLevelBounds(AffiliatedLevelPlane.PlaneSettings, false);
			for (int i = 0; i < 2; i++)
			{
				if (directionFlipCooldown[i] > 0)
				{
					directionFlipCooldown[i] -= Time.deltaTime;

					continue;
				}

				if (Mathf.Abs(normalizedPositionInLevel[i]) > 1.1f)
				{
					movementDirection[i] = Mathf.Abs(movementDirection[i]) * Mathf.Sign(normalizedPositionInLevel[i]) * -1;
				}

				directionFlipCooldown[i] = DIRECTION_FLIP_CHECK_COOLDOWN;
			}
		}

		private void CheckForContact()
		{
			if (!PlayerMover.Existent || (damageCooldown > 0) || (PlaneLevelIndex != LevelLoader.PlayerLevelIndex))
			{
				return;
			}

			CheckForEntityContact(PlayerMover.Instance);
		}

		private void CheckForEntityContact(EntityMover target)
		{
			int bodyPartCount = EntityFactory.GetTotalEntityBodyPartCount(target);
			for (int i = 0; i < bodyPartCount; i++)
			{
				if (CheckForContactOnPart(EntityFactory.GetBodyPartFromIndex(target, i), i / (float) bodyPartCount))
				{
					damageCooldown = obstacleSettings.DamageCooldown;

					break;
				}
			}

			bool CheckForContactOnPart(EntityBodyPart part, float toHeadDistance)
			{
				Vector2 dif = part.transform.position.XYZtoXZ() - transform.position.XYZtoXZ();
				float sqrDist = dif.sqrMagnitude;
				if (sqrDist < (obstacleSettings.DamageRange * obstacleSettings.DamageRange))
				{
					Vector2? impactDirection = null;
					if (obstacleSettings.DamageImpactForce != 0)
					{
						impactDirection = dif / Mathf.Sqrt(sqrDist);
						Vector2 impactForce = impactDirection.Value * obstacleSettings.DamageImpactForce;
						target.AddForceImpact(impactForce           * (1 - toHeadDistance));
						part.Body.AddForce(impactForce.XZtoXYZ()    * toHeadDistance);
					}

					if (obstacleSettings.DamageAmount > 0)
					{
						target.Damage(obstacleSettings.DamageAmount);
					}

					if (obstacleSettings.BounceOnDealDamage)
					{
						if (!impactDirection.HasValue)
						{
							impactDirection = dif / Mathf.Sqrt(sqrDist);
						}

						movementDirection = -impactDirection.Value;
					}

					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}