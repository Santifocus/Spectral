using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Spectral.Behaiviors
{
	public class PlayerBehavior : MovingEntitie
	{
		[SerializeField] private Camera playerViewCamera = default;
		protected override void Update()
		{
			base.Update();
			UpdateIntendedMoveDirection();
		}
		private void UpdateIntendedMoveDirection()
		{
			Vector3? inputPos = GetInputPosition();
			if (!inputPos.HasValue)
			{
				IntendedMoveDirection = null;
				return;
			}
			
			Vector2 posInputPosDif = inputPos.Value - playerViewCamera.WorldToScreenPoint(head.transform.position);
			posInputPosDif.x /= Screen.width;
			posInputPosDif.y /= Screen.height;
			intendedAcceleration = Mathf.Min(posInputPosDif.magnitude * 4, 1);

			if(intendedAcceleration < INTENDED_ACCELERATION_CUTOFF)
			{
				intendedAcceleration = 0;
				return;
			}

			Vector3 newIntendedMoveDirection = posInputPosDif.normalized;
			(newIntendedMoveDirection.y, newIntendedMoveDirection.z) = (newIntendedMoveDirection.z, newIntendedMoveDirection.y); //Swap y & z
			newIntendedMoveDirection.y = 0;

			IntendedMoveDirection = newIntendedMoveDirection;
		}
		private Vector3? GetInputPosition()
		{
			//Placeholder
			return Input.GetMouseButton(0) ? (Vector2?)Input.mousePosition : null;
		}
	}
}