using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class LevelBackgroundController : MonoBehaviour
	{
		[SerializeField] private Renderer targetRenderer = default;
		[SerializeField] private string materialColorName = "_BaseColor";

		private int colorNameID;

		private bool transitioning;
		private float colorLerpTime;
		private Color startColor;
		private Color targetColor;

		private void Start()
		{
			LevelLoader.LevelTransitionBegan += OnLevelTransitionStart;
			colorNameID = Shader.PropertyToID(materialColorName);
			startColor = targetColor = LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.BackgroundColor;
			UpdateBackgroundColor();
		}

		private void OnDestroy()
		{
			LevelLoader.LevelTransitionBegan -= OnLevelTransitionStart;
		}

		private void Update()
		{
			if (!transitioning)
			{
				return;
			}

			colorLerpTime += Time.deltaTime;
			UpdateBackgroundColor();
			if (colorLerpTime > LevelLoaderSettings.Current.LevelTransitionTime)
			{
				transitioning = false;
			}
		}

		private void OnLevelTransitionStart(int transitionDirection, LevelPlane previousLevelPlane, LevelPlane newLevelPlane)
		{
			transitioning = true;
			colorLerpTime = 0;
			startColor = previousLevelPlane.PlaneSettings.BackgroundColor;
			targetColor = newLevelPlane.PlaneSettings.BackgroundColor;
		}

		private void UpdateBackgroundColor()
		{
			targetRenderer.material.SetColor(colorNameID, Color.Lerp(startColor, targetColor, colorLerpTime / LevelLoaderSettings.Current.LevelTransitionTime));
		}
	}
}