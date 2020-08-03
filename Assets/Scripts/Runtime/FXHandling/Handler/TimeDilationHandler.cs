using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class TimeDilationHandler : FXHandler
	{
		public override TimeType UpdateStyle => TimeType.UnscaledDeltaTime;
		public override System.Type FXTargetType => typeof(TimeDilation);
		private readonly List<TimeDilationInstance> effectInstances = new List<TimeDilationInstance>();

		private float defaultFixedDeltaTime;

		public override void OnHandlerInitiated()
		{
			defaultFixedDeltaTime = Time.fixedDeltaTime;
		}

		public override FXInstance InitialiseFX(FXObject baseData, FXInstanceData instanceData)
		{
			TimeDilationInstance newInstance = new TimeDilationInstance(baseData, instanceData);
			effectInstances.Add(newInstance);

			return newInstance;
		}

		public override void HandleFX(float timeStep)
		{
			if (effectInstances.Count == 0)
			{
				return;
			}

			float highestDominance = float.MinValue;
			int highestDominanceIndex = -1;
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
					if (effectInstances[i].Dominance > highestDominance)
					{
						highestDominance = effectInstances[i].Dominance;
						highestDominanceIndex = i;
					}
				}
			}

			if (effectInstances.Count == 0)
			{
				Reset();

				return;
			}

			float targetTimeScale = effectInstances[highestDominanceIndex].GetTimeScale();
			Time.timeScale = targetTimeScale;
			Time.fixedDeltaTime = defaultFixedDeltaTime * targetTimeScale;
		}

		protected override IEnumerable<FXInstance> GetFXInstances()
		{
			foreach (TimeDilationInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset()
		{
			Time.fixedDeltaTime = defaultFixedDeltaTime;
			Time.timeScale = StaticData.GameIsPaused ? 0 : 1;
		}

		private class TimeDilationInstance : FXInstance<TimeDilation>
		{
			public float Dominance => FXData.Dominance;
			private float ScaleDifference => FXData.Scale - 1;
			public TimeDilationInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			public float GetTimeScale()
			{
				return 1 + (GetCurrentMultiplier() * ScaleDifference);
			}
		}
	}
}