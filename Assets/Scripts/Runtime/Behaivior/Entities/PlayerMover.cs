using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Spectral.Behaviours.Entities
{
	public class PlayerMover : EntityMover
	{
		private const float INTENDED_ACCELERATION_CUTOFF = 0.1f;
		public static PlayerMover Instance { get; private set; }
		public static bool Existant => Instance && Instance.alive;
		[SerializeField] private Camera playerViewCamera = default;
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
		}
		private void UpdateIntendedMoveDirection()
		{
			Vector2? inputPos = GetInputPosition();
			if (!inputPos.HasValue)
			{
				IntendedMoveDirection = null;
				intendedAcceleration = 0;
				return;
			}

			inputPos = new Vector2(Mathf.Clamp(inputPos.Value.x, 0, Screen.width), Mathf.Clamp(inputPos.Value.y, 0, Screen.height));
			Vector2 posInputPosDif = inputPos.Value - (Vector2)playerViewCamera.WorldToScreenPoint(Head.transform.position);
			posInputPosDif.x /= Screen.width;
			posInputPosDif.y /= Screen.height;
			intendedAcceleration = Mathf.Min(posInputPosDif.magnitude * 3, 1);

			if (intendedAcceleration < INTENDED_ACCELERATION_CUTOFF)
			{
				intendedAcceleration = 0;
				return;
			}

			Vector3 newIntendedMoveDirection = posInputPosDif.normalized;
			(newIntendedMoveDirection.y, newIntendedMoveDirection.z) = (newIntendedMoveDirection.z, newIntendedMoveDirection.y);
			newIntendedMoveDirection.y = 0;

			IntendedMoveDirection = newIntendedMoveDirection;
		}
		private void CheckForFood()
		{
			FoodObject targetFood = FoodObject.GetNearestFoodObject(Head.transform.position.XYZtoXZ(), EatDistance);
			if (targetFood)
			{
				targetFood.Eat(this);
			}
		}
		private Vector2? GetInputPosition()
		{
#if UNITY_ANDROID || UNITY_IPHONE

			return Input.touchCount > 0 ? (Vector2?)Input.GetTouch(0).position : null;
#else
			return Input.GetMouseButton(0) ? (Vector2?)Input.mousePosition : null;
#endif
		}
	}
}