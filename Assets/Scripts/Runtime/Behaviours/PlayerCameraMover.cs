using Spectral.Runtime.Behaviours.Entities;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	[RequireComponent(typeof(Camera))]
	public class PlayerCameraMover : MonoBehaviour
	{
		private static PlayerCameraMover activePlayerCamera { get; set; }
		public static Camera ActiveCamera => activePlayerCamera.cameraComponent;
		public static Transform ShakeCore => activePlayerCamera.shakeCore;

		[SerializeField] private float toPlayerOffset = 25;
		[SerializeField] private float followSpeed = 2;
		[SerializeField] private float scaleLerpSpeed = 0.5f;
		[SerializeField] private float levelDimensionLerp = 0.5f;
		[SerializeField] private Transform shakeCore = default;

		private bool initiated;
		private Camera cameraComponent;
		private Vector2 currentBasePosition;
		private float currentToPlayerOffset;

		private float currentlyUsedLevelWidth;
		private float currentlyUsedLevelHeight;

		private void Start()
		{
			activePlayerCamera = this;
			cameraComponent = GetComponent<Camera>();
			currentToPlayerOffset = toPlayerOffset;
		}

		private void Initialise()
		{
			initiated = true;
			currentToPlayerOffset = toPlayerOffset * PlayerMover.Instance.Head.transform.localScale.z;
			currentBasePosition = PlayerMover.Instance.Head.transform.position.XYZtoXZ();
		}

		private void FixedUpdate()
		{
			if (!PlayerMover.Existent)
			{
				return;
			}

			if (!initiated)
			{
				Initialise();
			}

			//Update zoom
			currentToPlayerOffset = Mathf.Lerp(currentToPlayerOffset, toPlayerOffset * PlayerMover.Instance.Head.transform.localScale.z, Time.fixedDeltaTime * scaleLerpSpeed);

			//Calculate position of the Camera relative to the player
			Vector2 basePosition = GetCurrentBasePosition();
			Vector2 viewRect = CalculateViewRect();
			UpdateUsedLevelDimensions(viewRect);

			//Normalize position based on the level-bounds minus the view rect
			Vector2 normalizedPosition = NormalizePositionFromLevelBoundsAndViewRect(basePosition, currentlyUsedLevelWidth, currentlyUsedLevelHeight);
			transform.position = new Vector3(currentlyUsedLevelWidth * normalizedPosition.x, currentToPlayerOffset, currentlyUsedLevelHeight * normalizedPosition.y);
		}

		private void UpdateUsedLevelDimensions(Vector2 viewRect)
		{
			float levelWidth = (LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.LevelWidth   / 2) - viewRect.x;
			float levelHeight = (LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.LevelHeight / 2) - viewRect.y;
			currentlyUsedLevelWidth = Mathf.Lerp(currentlyUsedLevelWidth, levelWidth, levelDimensionLerp    * Time.deltaTime);
			currentlyUsedLevelHeight = Mathf.Lerp(currentlyUsedLevelHeight, levelHeight, levelDimensionLerp * Time.deltaTime);
		}

		private Vector2 GetCurrentBasePosition()
		{
			currentBasePosition = Vector2.Lerp(currentBasePosition, PlayerMover.Instance.Head.transform.position.XYZtoXZ(), followSpeed * Time.fixedDeltaTime);

			return currentBasePosition;
		}

		private Vector2 CalculateViewRect()
		{
			float verticalFOV = cameraComponent.fieldOfView                                                                   / 2;
			float horizontalFOV = Camera.VerticalToHorizontalFieldOfView(cameraComponent.fieldOfView, cameraComponent.aspect) / 2;

			return new Vector2((currentToPlayerOffset  / Mathf.Cos(horizontalFOV * Mathf.Deg2Rad)) * Mathf.Sin(horizontalFOV * Mathf.Deg2Rad),
								(currentToPlayerOffset / Mathf.Cos(verticalFOV   * Mathf.Deg2Rad)) * Mathf.Sin(verticalFOV   * Mathf.Deg2Rad));
		}

		public Vector2 NormalizePositionFromLevelBoundsAndViewRect(Vector2 playerPosition, float levelWidth, float levelHeight)
		{
			return new Vector2(levelWidth   <= 0 ? 0 : Mathf.Clamp(playerPosition.x / levelWidth, -1, 1),
								levelHeight <= 0 ? 0 : Mathf.Clamp(playerPosition.y / levelHeight, -1, 1));
		}

		public static void EnsureResetShakeCore()
		{
			if (ShakeCore.localPosition.sqrMagnitude > 0.01f)
			{
				ShakeCore.localPosition = Vector3.zero;
			}
		}
	}
}