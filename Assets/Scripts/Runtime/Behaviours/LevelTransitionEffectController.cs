using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class LevelTransitionEffectController : MonoBehaviour
	{
		[Header("Background Coloring")] [SerializeField]
		private Renderer backgroundRenderer = default;

		[SerializeField] private string backgroundColorValueName = default;

		[Header("Transition Effect")] [SerializeField]
		private Renderer transitionEffectRenderer = default;

		[SerializeField] private string transitionEffectValueName = default;
		[SerializeField] private string transitionEffectDirectionValueName = default;
		[SerializeField] private float transitionEffectFinishDelay = -0.25f;
		[SerializeField] private float transitionEffectMaxIntensity = 1;

		private int backgroundColorNameID;
		private int transitionEffectNameID;
		private int transitionEffectDirectionNameID;

		private bool transitioning;
		private float lerpTime;
		private Color backgroundColorStart;
		private Color backgroundColorTarget;

		private void Start()
		{
			LevelLoader.LevelTransitionBegan += OnLevelTransitionStart;

			//Cache Property IDs
			backgroundColorNameID = Shader.PropertyToID(backgroundColorValueName);
			transitionEffectNameID = Shader.PropertyToID(transitionEffectValueName);
			transitionEffectDirectionNameID = Shader.PropertyToID(transitionEffectDirectionValueName);

			//Use the current level plane as base for the background colors
			backgroundColorStart = backgroundColorTarget = LevelLoader.GameLevelPlanes[LevelLoader.PlayerLevelIndex].PlaneSettings.BackgroundColor;

			//Reset any Editor set values to the base state
			FinaliseColorTransition();
			FinaliseTransitionEffect();
		}

		private void OnDestroy()
		{
			LevelLoader.LevelTransitionBegan -= OnLevelTransitionStart;
		}

		private void OnLevelTransitionStart(int transitionDirection, LevelPlane previousLevelPlane, LevelPlane newLevelPlane, bool hasTransitionedToPlaneBefore)
		{
			transitioning = true;
			lerpTime = 0;
			InitialiseColorTransition(previousLevelPlane, newLevelPlane);
			InitialiseTransitionEffect(transitionDirection);
		}

		private void InitialiseColorTransition(LevelPlane previousLevelPlane, LevelPlane newLevelPlane)
		{
			backgroundColorStart = previousLevelPlane.PlaneSettings.BackgroundColor;
			backgroundColorTarget = newLevelPlane.PlaneSettings.BackgroundColor;
		}

		private void InitialiseTransitionEffect(int transitionDirection)
		{
			transitionEffectRenderer.material.SetFloat(transitionEffectDirectionNameID, transitionDirection);
			transitionEffectRenderer.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (!transitioning)
			{
				return;
			}

			lerpTime += Time.deltaTime;
			if (lerpTime > Mathf.Max(LevelLoaderSettings.Current.LevelTransitionTime, LevelLoaderSettings.Current.LevelTransitionTime + transitionEffectFinishDelay))
			{
				FinaliseColorTransition();
				FinaliseTransitionEffect();
				transitioning = false;
			}
			else
			{
				UpdateColorTransition();
				UpdateTransitionEffect();
			}
		}

		private void UpdateColorTransition()
		{
			backgroundRenderer.material.SetColor(backgroundColorNameID,
												Color.Lerp(backgroundColorStart, backgroundColorTarget, lerpTime / LevelLoaderSettings.Current.LevelTransitionTime));
		}

		private void UpdateTransitionEffect()
		{
			if (PlayerMover.Existent)
			{
				//Update the renderer position
				transitionEffectRenderer.transform.position = PlayerMover.Instance.Head.transform.position;
			}

			//Set the transition effect value
			float lerpPoint = Mathf.Sin(Mathf.PI * (lerpTime / (LevelLoaderSettings.Current.LevelTransitionTime + transitionEffectFinishDelay)));
			transitionEffectRenderer.material.SetFloat(transitionEffectNameID, lerpPoint * transitionEffectMaxIntensity);
		}

		private void FinaliseColorTransition()
		{
			backgroundRenderer.material.SetColor(backgroundColorNameID, backgroundColorTarget);
		}

		private void FinaliseTransitionEffect()
		{
			transitionEffectRenderer.material.SetFloat(transitionEffectNameID, 0);
			transitionEffectRenderer.gameObject.SetActive(false);
		}
	}
}