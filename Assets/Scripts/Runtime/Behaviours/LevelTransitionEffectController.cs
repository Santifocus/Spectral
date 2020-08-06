using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class LevelTransitionEffectController : MonoBehaviour
	{
		[Header("Transition Effect")] [SerializeField]
		private Renderer transitionEffectRenderer = default;

		[SerializeField] private string transitionEffectValueName = default;
		[SerializeField] private string transitionEffectDirectionValueName = default;
		[SerializeField] private float transitionEffectFinishDelay = -0.25f;
		[SerializeField] private float transitionEffectMaxIntensity = 1;

		private int transitionEffectNameID;
		private int transitionEffectDirectionNameID;

		private bool transitioning;
		private float lerpTime;

		private void Start()
		{
			LevelLoader.LevelTransitionBegan += OnLevelTransitionStart;

			//Cache Property IDs
			transitionEffectNameID = Shader.PropertyToID(transitionEffectValueName);
			transitionEffectDirectionNameID = Shader.PropertyToID(transitionEffectDirectionValueName);

			//Reset any Editor set values to the base state
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
			InitialiseTransitionEffect(transitionDirection);
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
				FinaliseTransitionEffect();
				transitioning = false;
			}
			else
			{
				UpdateTransitionEffect();
			}
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

		private void FinaliseTransitionEffect()
		{
			transitionEffectRenderer.material.SetFloat(transitionEffectNameID, 0);
			transitionEffectRenderer.gameObject.SetActive(false);
		}
	}
}