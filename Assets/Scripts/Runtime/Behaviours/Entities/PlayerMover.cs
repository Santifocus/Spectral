using System;
using UnityEngine;

namespace Spectral.Runtime.Behaviours.Entities
{
	public class PlayerMover : EntityMover
	{
		private const float INTENDED_ACCELERATION_CUTOFF = 0.1f;
		public static event Action PlayerAte;
		public static event Action PlayerTookDamage;
		public static event Action PlayerDied;
		public static PlayerMover Instance { get; private set; }
		public static bool Existent => Instance && Instance.Alive;

		protected override void Start()
		{
			base.Start();
			Instance = this;
		}

		protected override void Update()
		{
			base.Update();
			UpdateIntendedMoveDirection();
			CheckForFood();
			ApplyLevelBorderForceField();
		}

		private void UpdateIntendedMoveDirection()
		{
			Vector2? inputPos = GetInputPosition();
			if (!inputPos.HasValue)
			{
				IntendedMoveDirection = null;
				IntendedAcceleration = 0;

				return;
			}

			inputPos = new Vector2(Mathf.Clamp(inputPos.Value.x, 0, Screen.width), Mathf.Clamp(inputPos.Value.y, 0, Screen.height));
			Vector2 posInputPosDif = inputPos.Value - (Vector2) PlayerCameraMover.ActiveCamera.WorldToScreenPoint(Head.transform.position);
			posInputPosDif.x /= Screen.width;
			posInputPosDif.y /= Screen.height;
			IntendedAcceleration = Mathf.Min(posInputPosDif.magnitude * 3, 1);
			if (IntendedAcceleration < INTENDED_ACCELERATION_CUTOFF)
			{
				IntendedAcceleration = 0;

				return;
			}

			Vector3 newIntendedMoveDirection = posInputPosDif.normalized;
			(newIntendedMoveDirection.y, newIntendedMoveDirection.z) = (newIntendedMoveDirection.z, newIntendedMoveDirection.y);
			newIntendedMoveDirection.y = 0;
			IntendedMoveDirection = newIntendedMoveDirection;
		}

		private void CheckForFood()
		{
			FoodObject targetFood = FoodSpawner.GetNearestFoodObject(Head.transform.position.XYZtoXZ(), LevelLoader.PlayerLevelIndex, EatDistance);
			if (targetFood)
			{
				OnEat(targetFood);
			}
		}

		private void ApplyLevelBorderForceField()
		{
			float levelWidth = LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.LevelWidth   / 2;
			float levelHeight = LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.LevelHeight / 2;
			Vector2 selfPosition = Head.transform.position.XYZtoXZ();
			Vector2 diff = new Vector2(levelWidth - Mathf.Abs(selfPosition.x), levelHeight - Mathf.Abs(selfPosition.y));
			Vector2 forceImpactVector = new Vector2(Mathf.Clamp01(1 - (diff.x / GameSettings.Current.LevelBorderForceFieldWidth)),
													Mathf.Clamp01(1 - (diff.y / GameSettings.Current.LevelBorderForceFieldWidth)));

			float forceImpact = forceImpactVector.Sum() * GameSettings.Current.LevelBorderForceFieldStrength;
			if (forceImpact > 0.01f)
			{
				Vector2 direction = new Vector2((forceImpactVector.x > 0 ? 1 : 0) * Mathf.Sign(selfPosition.x * -1),
												(forceImpactVector.y > 0 ? 1 : 0) * Mathf.Sign(selfPosition.y * -1)).normalized;

				AddForceImpact(forceImpact * Time.deltaTime * direction);
			}
		}

		private Vector2? GetInputPosition()
		{
#if UNITY_ANDROID || UNITY_IPHONE
			return Input.touchCount > 0 ? (Vector2?)Input.GetTouch(0).position : null;
#else
			return Input.GetMouseButton(0) ? (Vector2?) Input.mousePosition : null;
#endif
		}

		public override void OnEat(FoodObject target = null)
		{
			base.OnEat(target);
			PlayerAte?.Invoke();
		}

		public override void Damage(int amount = 1)
		{
			base.Damage(amount);
			PlayerTookDamage?.Invoke();
		}

		public override void Death()
		{
			base.Death();
			PlayerDied?.Invoke();
		}
	}
}