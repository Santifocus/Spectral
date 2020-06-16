using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class ScreenBorderColorHandler : FXHandler
	{
		private static readonly int ShaderColor = Shader.PropertyToID("_Color");
		private static readonly int ShaderBorderDepth = Shader.PropertyToID("_BorderDepth");
		public override TimeType UpdateStyle => TimeType.ScaledDeltaTime;
		public override System.Type FXTargetType => typeof(ScreenBorderColor);
		private readonly List<ScreenBorderColorInstance> effectInstances = new List<ScreenBorderColorInstance>();

		public override FXInstance InitiateFX(FXObject baseData, FXInstanceData instanceData)
		{
			ScreenBorderColorInstance newInstance = new ScreenBorderColorInstance(baseData, instanceData);
			effectInstances.Add(newInstance);

			return newInstance;
		}

		public override void HandleFX(float timeStep)
		{
			if (effectInstances.Count == 0)
			{
				return;
			}

			float averageDepth = 0;
			Color averageColor = Color.clear;
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
					averageDepth += effectInstances[i].GetDepth();
					averageColor += effectInstances[i].BorderColor;
				}
			}

			if (effectInstances.Count == 0)
			{
				Reset();

				return;
			}

			averageDepth /= effectInstances.Count;
			averageColor /= effectInstances.Count;
			GameSettings.Current.ScreenEffectMaterial.SetFloat(ShaderBorderDepth, averageDepth);
			GameSettings.Current.ScreenEffectMaterial.SetColor(ShaderColor, averageColor);
		}

		protected override IEnumerable<FXInstance> GetFXInstances()
		{
			foreach (ScreenBorderColorInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset()
		{
			GameSettings.Current.ScreenEffectMaterial.SetFloat(ShaderBorderDepth, 0);
			GameSettings.Current.ScreenEffectMaterial.SetColor(ShaderColor, Color.clear);
		}

		private class ScreenBorderColorInstance : FXInstance<ScreenBorderColor>
		{
			public Color BorderColor => FXData.Color;
			public ScreenBorderColorInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			public float GetDepth()
			{
				return GetCurrentMultiplier() * FXData.Depth;
			}
		}
	}
}