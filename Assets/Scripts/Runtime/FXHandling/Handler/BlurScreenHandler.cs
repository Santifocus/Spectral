using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class BlurScreenHandler : FXHandler
	{
		private const float BLUR_LERP_SPEED = 8;
		private static readonly int ShaderBlurRange = Shader.PropertyToID("_BlurRange");
		private static readonly int ShaderBlurPower = Shader.PropertyToID("_BlurPower");
		public override TimeType UpdateStyle => TimeType.ScaledDeltaTime;
		public override System.Type FXTargetType => typeof(ScreenBlur);
		private readonly List<ScreenBlurInstance> effectInstances = new List<ScreenBlurInstance>();

		private float curBlurIntensity;
		private float curBlurDistance;

		public override FXInstance InitialiseFX(FXObject baseData, FXInstanceData instanceData)
		{
			ScreenBlurInstance newInstance = new ScreenBlurInstance(baseData, instanceData);
			effectInstances.Add(newInstance);

			return newInstance;
		}

		public override void HandleFX(float timeStep)
		{
			if (effectInstances.Count == 0)
			{
				return;
			}

			float strongestIntensity = 0;
			int furthestBlurDistance = 0;
			for (int i = 0; i < effectInstances.Count; i++)
			{
				effectInstances[i].Update(timeStep);
				if (effectInstances[i].WillBeDestroyed())
				{
					effectInstances.RemoveAt(i);
					i--;
				}
				else
				{
					(float intensity, int blurDistance) = effectInstances[i].GetIntensityAndDistance();
					if (intensity > strongestIntensity)
					{
						strongestIntensity = intensity;
					}

					if (blurDistance > furthestBlurDistance)
					{
						furthestBlurDistance = blurDistance;
					}
				}
			}

			if (effectInstances.Count == 0)
			{
				Reset();

				return;
			}

			curBlurIntensity = Mathf.Lerp(curBlurIntensity, strongestIntensity, timeStep * BLUR_LERP_SPEED);
			curBlurDistance = Mathf.Lerp(curBlurDistance, furthestBlurDistance, timeStep * BLUR_LERP_SPEED);
			GameSettings.Current.ScreenEffectMaterial.SetFloat(ShaderBlurPower, curBlurIntensity);
			GameSettings.Current.ScreenEffectMaterial.SetInt(ShaderBlurRange, Mathf.RoundToInt(curBlurDistance));
		}

		protected override IEnumerable<FXInstance> GetFXInstances()
		{
			foreach (ScreenBlurInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset()
		{
			curBlurIntensity = 0;
			curBlurDistance = 0;
			GameSettings.Current.ScreenEffectMaterial.SetFloat(ShaderBlurPower, 0);
			GameSettings.Current.ScreenEffectMaterial.SetInt(ShaderBlurRange, 0);
		}

		private class ScreenBlurInstance : FXInstance<ScreenBlur>
		{
			public ScreenBlurInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			public (float, int) GetIntensityAndDistance()
			{
				float multiplier = GetCurrentMultiplier();

				return (FXData.Intensity * multiplier, Mathf.RoundToInt(FXData.BlurDistance * multiplier));
			}
		}
	}
}