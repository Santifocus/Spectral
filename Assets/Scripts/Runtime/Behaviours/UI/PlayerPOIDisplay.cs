using UnityEngine;
using UnityEngine.UI;

namespace Spectral.Runtime.Behaviours.UI
{
	public class PlayerPOIDisplay : MonoBehaviour
	{
		[Header("Pointer")] [SerializeField] private RectTransform onScreenPointer = default;
		[SerializeField] private RectTransform offScreenPointerCore = default;
		[SerializeField] private Vector2 borderPadding = new Vector2(24, 24);

		[Header("Pointer Coloring")] [SerializeField]
		private Graphic[] pointerComponents = default;

		[SerializeField] private Color activePointerColor = Color.green / 2;
		[SerializeField] private Color inActivePointerColor = Color.clear;
		[SerializeField] private float colorTransitionTime = 0.5f;

		private float colorTransitionTimer;
		private bool gateOnScreen;

		private void Start()
		{
			colorTransitionTimer = TransitionGateActive() ? colorTransitionTime : 0;
			UpdatePointerPosition();
			UpdatePointerColor();
			SetGateOnScreenState(false);
		}

		private void Update()
		{
			UpdatePointerPosition();
			UpdatePointerColor();
		}

		private void UpdatePointerPosition()
		{
			if ((LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].CoreObject                       == null)
				|| (LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].CoreObject.downTransitionGate == null))
			{
				return;
			}

			Vector3 gateOnScreenPoint = PlayerCameraMover.ActiveCamera.WorldToScreenPoint(LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].CoreObject.downTransitionGate
																									.transform.position);

			UpdateIfGateOnScreen(gateOnScreenPoint);
			if (gateOnScreen)
			{
				onScreenPointer.position = (Vector2) gateOnScreenPoint;
			}
			else
			{
				Vector2 clampedOnScreen = new Vector2(Mathf.Clamp(gateOnScreenPoint.x, borderPadding.x, Screen.width - borderPadding.x),
													Mathf.Clamp(gateOnScreenPoint.y, borderPadding.y, Screen.height  - borderPadding.y));

				offScreenPointerCore.position = clampedOnScreen;
				Vector2 clampDif = (Vector2) gateOnScreenPoint - clampedOnScreen;
				float pointerAngle = Mathf.Atan2(clampDif.y, clampDif.x) * Mathf.Rad2Deg;
				offScreenPointerCore.localEulerAngles = new Vector3(0, 0, pointerAngle);
			}
		}

		private void UpdateIfGateOnScreen(Vector3 gateOnScreenPoint)
		{
			bool curGateOnScreen = (gateOnScreenPoint.z  > 0) &&
									(gateOnScreenPoint.x > 0) && (gateOnScreenPoint.x < Screen.width) &&
									(gateOnScreenPoint.y > 0) && (gateOnScreenPoint.y < Screen.height);

			if (curGateOnScreen != gateOnScreen)
			{
				SetGateOnScreenState(curGateOnScreen);
			}
		}

		private void SetGateOnScreenState(bool newState)
		{
			gateOnScreen = newState;
			onScreenPointer.gameObject.SetActive(gateOnScreen);
			offScreenPointerCore.gameObject.SetActive(!gateOnScreen);
		}

		private void UpdatePointerColor()
		{
			colorTransitionTimer = Mathf.Clamp(colorTransitionTimer + (Time.deltaTime * (TransitionGateActive() ? 1 : -1)), 0, colorTransitionTime);
			float lerpPoint = colorTransitionTimer / colorTransitionTime;
			foreach (Graphic pointerComponent in pointerComponents)
			{
				pointerComponent.color = Color.Lerp(inActivePointerColor, activePointerColor, lerpPoint);
			}
		}

		private bool TransitionGateActive()
		{
			return LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].CoreObject                     &&
					LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].CoreObject.downTransitionGate &&
					LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].CoreObject.downTransitionGate.CanBeActivated();
		}
	}
}