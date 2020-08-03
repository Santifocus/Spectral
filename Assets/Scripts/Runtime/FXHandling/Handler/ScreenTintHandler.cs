using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class ScreenTintHandler : FXHandler
	{
		private static readonly int ShaderTintColor = Shader.PropertyToID("_TintColor");
		public override TimeType UpdateStyle => TimeType.ScaledDeltaTime;
		public override System.Type FXTargetType => typeof(ScreenTint);
		private readonly List<ScreenTintInstance> effectInstances = new List<ScreenTintInstance>();

		public override FXInstance InitialiseFX(FXObject baseData, FXInstanceData instanceData)
		{
			ScreenTintInstance newInstance = new ScreenTintInstance(baseData, instanceData);
			effectInstances.Add(newInstance);

			return newInstance;
		}

		public override void HandleFX(float timeStep)
		{
			if (effectInstances.Count == 0)
			{
				return;
			}

			float totalDominanceAmount = 0;
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
					totalDominanceAmount += effectInstances[i].Dominance;
				}
			}

			if (effectInstances.Count == 0)
			{
				Reset();

				return;
			}

			if (totalDominanceAmount <= 0)
			{
				return;
			}

			Color averageColor = Color.clear;
			foreach (ScreenTintInstance instance in effectInstances)
			{
				averageColor += instance.GetCurrentColor() * (instance.Dominance / totalDominanceAmount);
			}

			GameSettings.Current.ScreenEffectMaterial.SetColor(ShaderTintColor, averageColor);
		}

		protected override IEnumerable<FXInstance> GetFXInstances()
		{
			foreach (ScreenTintInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset()
		{
			GameSettings.Current.ScreenEffectMaterial.SetColor(ShaderTintColor, Color.clear);
		}

		private class ScreenTintInstance : FXInstance<ScreenTint>
		{
			public float Dominance => FXData.Dominance * GetCurrentMultiplier();
			public ScreenTintInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			public Color GetCurrentColor()
			{
				return new Color(FXData.TintColor.r, FXData.TintColor.g, FXData.TintColor.b, FXData.TintColor.a * GetCurrentMultiplier());
			}
		}
	}
}