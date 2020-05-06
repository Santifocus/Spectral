using Spectral.Behaviours.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaviours
{
	[RequireComponent(typeof(Camera))]
	public class PlayerCameraMover : SpectralMonoBehavior
	{
		[SerializeField] private float toPlayerOffset = 25;
		private Camera self;
		private void Start()
		{
			self = GetComponent<Camera>();
		}
		private void LateUpdate()
		{
			if (!PlayerMover.Existant)
			{
				return;
			}

			Vector3 basePosition = PlayerMover.Instance.Head.transform.position;
			Vector2 viewPort = CalculateViewPort();
			Vector2 offSetLerpValues = NormalizePositionFromLevelBoundsAndViewPort(basePosition.XYZtoXZ(), viewPort);

			transform.position = basePosition - new Vector3(viewPort.x * offSetLerpValues.x / 2, -toPlayerOffset, viewPort.y * offSetLerpValues.y / 2);
		}
		private Vector2 CalculateViewPort()
		{
			float verticalFOV = self.fieldOfView;
			float horizontalFOV = Camera.VerticalToHorizontalFieldOfView(verticalFOV, self.aspect);

			return new Vector2(Mathf.Sin(horizontalFOV / 2 * Mathf.Deg2Rad) * toPlayerOffset, Mathf.Sin(verticalFOV / 2 * Mathf.Deg2Rad) * toPlayerOffset) * 2;
		}
		public Vector2 NormalizePositionFromLevelBoundsAndViewPort(Vector2 self, Vector2 viewPort)
		{
			float levelWidht = GameManager.CurrentLevelSettings.LevelWidht.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelWidht) - viewPort.x;
			float levelHeight = GameManager.CurrentLevelSettings.LevelHeight.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelHeight) - viewPort.y;

			return new Vector2(self.x / (levelWidht / 2), self.y / (levelHeight / 2));
		}
	}
}