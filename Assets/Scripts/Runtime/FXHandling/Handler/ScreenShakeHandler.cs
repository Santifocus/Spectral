using System.Collections.Generic;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class ScreenShakeHandler : FXHandler
	{
		private const float DELAY_PER_SHAKE = 0.05f;
		public override TimeType UpdateStyle => TimeType.ScaledDeltaTime;
		public override System.Type FXTargetType => typeof(ScreenShake);
		private readonly List<ScreenShakeInstance> effectInstances = new List<ScreenShakeInstance>();

		private float delayTillNextShake;

		public override FXInstance InitialiseFX(FXObject baseData, FXInstanceData instanceData)
		{
			ScreenShakeInstance newInstance = new ScreenShakeInstance(baseData, instanceData);
			effectInstances.Add(newInstance);

			return newInstance;
		}

		public override void HandleFX(float timeStep)
		{
			delayTillNextShake -= timeStep;
			if (delayTillNextShake <= 0)
			{
				delayTillNextShake = DELAY_PER_SHAKE;
				UpdateShake();
			}
		}

		private void UpdateShake()
		{
			if (effectInstances.Count == 0)
			{
				Reset();

				return;
			}

			float strongestAxisIntensity = 0;
			Vector3 strongestAxisIntensityVector = Vector3.zero;
			float strongestAngleIntensity = 0;
			Vector3 strongestAngleIntensityVector = Vector3.zero;
			for (int i = 0; i < effectInstances.Count; i++)
			{
				effectInstances[i].Update(DELAY_PER_SHAKE);
				if (effectInstances[i].WillBeDestroyed())
				{
					effectInstances.RemoveAt(i);
					i--;
				}
				else
				{
					(Vector3 axisIntensityVector, Vector3 angleIntensityVector) = effectInstances[i].GetIntensities();
					float axisIntensity = axisIntensityVector.sqrMagnitude;
					if (axisIntensity > strongestAxisIntensity)
					{
						strongestAxisIntensity = axisIntensity;
						strongestAxisIntensityVector = axisIntensityVector;
					}

					float angleIntensity = angleIntensityVector.sqrMagnitude;
					if (angleIntensity > strongestAngleIntensity)
					{
						strongestAngleIntensity = angleIntensity;
						strongestAngleIntensityVector = angleIntensityVector;
					}
				}
			}

			ShakeScreen(strongestAxisIntensityVector, strongestAngleIntensityVector);
		}

		private void ShakeScreen(Vector3 axisIntensity, Vector3 angleIntensity)
		{
			axisIntensity *= Time.timeScale;
			angleIntensity *= Time.timeScale;
			PlayerCameraMover.ShakeCore.localPosition = (PlayerCameraMover.ShakeCore.right   * ((Random.value - 0.5f) * axisIntensity.x)) +
														(PlayerCameraMover.ShakeCore.up      * ((Random.value - 0.5f) * axisIntensity.y)) +
														(PlayerCameraMover.ShakeCore.forward * ((Random.value - 0.5f) * axisIntensity.z));

			PlayerCameraMover.ShakeCore.localEulerAngles = new Vector3((Random.value  - 0.5f) * angleIntensity.x,
																		(Random.value - 0.5f) * angleIntensity.y,
																		(Random.value - 0.5f) * angleIntensity.z);
		}

		protected override IEnumerable<FXInstance> GetFXInstances()
		{
			foreach (ScreenShakeInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset()
		{
			PlayerCameraMover.EnsureResetShakeCore();
		}

		private class ScreenShakeInstance : FXInstance<ScreenShake>
		{
			public ScreenShakeInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			public (Vector3, Vector3) GetIntensities()
			{
				float multiplier = GetCurrentMultiplier();

				return (FXData.CustomAxisMultiplier  * (FXData.AxisIntensity  * multiplier),
						FXData.CustomAngleMultiplier * (FXData.AngleIntensity * multiplier));
			}
		}
	}
}