using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class ParticleEffectHandler : FXHandler
	{
		public override TimeType UpdateStyle => TimeType.ScaledDeltaTime;
		public override System.Type FXTargetType => typeof(ParticleEffect);
		private readonly List<ParticleEffectInstance> effectInstances = new List<ParticleEffectInstance>();

		public override FXInstance InitiateFX(FXObject baseData, FXInstanceData instanceData)
		{
			ParticleEffectInstance newInstance = new ParticleEffectInstance(baseData, instanceData);
			effectInstances.Add(newInstance);

			return newInstance;
		}

		public override void HandleFX(float timeStep)
		{
			for (int i = 0; i < effectInstances.Count; i++)
			{
				effectInstances[i].Update(timeStep);
				if (effectInstances[i].WillBeDestroyed())
				{
					effectInstances.RemoveAt(i);
					i--;
				}
			}
		}

		protected override IEnumerable<FXInstance> GetFXInstances()
		{
			foreach (ParticleEffectInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset() { }

		private class ParticleEffectInstance : FXInstance<ParticleEffect>
		{
			private ParticleEffectInstanceData effectInstanceData;
			private float curMultiplier;

			public ParticleEffectInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			protected override void OnInitiate()
			{
				base.OnInitiate();

				//Read the transform overwrite data
				Vector3 positionOffset = InstanceData.CustomPositionOffset ?? FXData.OffsetToTarget;
				Vector3 rotationOffset = InstanceData.CustomRotationOffset ?? FXData.RotationOffset;
				Vector3 scale = InstanceData.CustomScale                   ?? FXData.ParticleMainObject.transform.localScale;

				//Create the main object
				Transform mainObjectTransform = Object.Instantiate(FXData.ParticleMainObject, Storage.ParticleStorage).transform;
				mainObjectTransform.position = InstanceData.Parent.position       + positionOffset;
				mainObjectTransform.eulerAngles = InstanceData.Parent.eulerAngles + rotationOffset;
				mainObjectTransform.localScale = scale;

				//Record the base states of the particle systems
				ParticleSystem[] containedSystems = mainObjectTransform.GetComponentsInChildren<ParticleSystem>();
				float[] emissionBasis = new float[containedSystems.Length];
				for (int i = 0; i < containedSystems.Length; i++)
				{
					if (containedSystems[i].isPlaying)
					{
						containedSystems[i].Stop();
					}

					containedSystems[i].Play();
					emissionBasis[i] = containedSystems[i].emission.rateOverTimeMultiplier;
				}

				effectInstanceData = new ParticleEffectInstanceData(mainObjectTransform, containedSystems, emissionBasis, positionOffset, rotationOffset);
				SetEmissionMultiplier(0);
			}

			protected override void InternalUpdate(float timeStep)
			{
				UpdateTransform(timeStep);
				UpdateEmission();
			}

			private void UpdateTransform(float timeStep)
			{
				if (!FXData.FollowTarget || !InstanceData.Parent)
				{
					return;
				}

				//Update position
				effectInstanceData.MainObjectTransform.position = InstanceData.Parent.position + InstanceData.Parent.TransformVector(effectInstanceData.PositionOffset);

				//Update rotation
				if (FXData.InheritRotationOfTarget)
				{
					effectInstanceData.MainObjectTransform.rotation = Quaternion.Lerp(effectInstanceData.MainObjectTransform.rotation, InstanceData.Parent.transform.rotation,
																					timeStep * FXData
																						.RotationInheritLerpSpeed);

					effectInstanceData.MainObjectTransform.transform.eulerAngles += effectInstanceData.RotationOffset;
				}
			}

			private void UpdateEmission()
			{
				float newMultiplier = GetCurrentMultiplier();
				if (System.Math.Abs(newMultiplier - curMultiplier) > 0.01f)
				{
					SetEmissionMultiplier(newMultiplier);
				}
			}

			private void SetEmissionMultiplier(float newMultiplier)
			{
				curMultiplier = newMultiplier;
				if ((CurrentFXState == FXState.Ending) && (newMultiplier < 0.01f))
				{
					foreach (ParticleSystem particleSystem in effectInstanceData.ContainedSystems)
					{
						particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
					}
				}
				else
				{
					for (int i = 0; i < effectInstanceData.ContainedSystems.Length; i++)
					{
						ParticleSystem.EmissionModule em = effectInstanceData.ContainedSystems[i].emission;
						em.rateOverTimeMultiplier = newMultiplier * effectInstanceData.EmissionBasis[i];
					}
				}
			}

			protected override bool AllowedToDestroy()
			{
				foreach (ParticleSystem particleSystem in effectInstanceData.ContainedSystems)
				{
					if (particleSystem.particleCount > 0)
					{
						return false;
					}
				}

				return true;
			}

			protected override void OnDestroy()
			{
				Object.Destroy(effectInstanceData.MainObjectTransform.gameObject);
			}

			private readonly struct ParticleEffectInstanceData
			{
				public readonly Transform MainObjectTransform;

				public readonly ParticleSystem[] ContainedSystems;
				public readonly float[] EmissionBasis;

				public readonly Vector3 PositionOffset;
				public readonly Vector3 RotationOffset;

				public ParticleEffectInstanceData(Transform mainObjectTransform, ParticleSystem[] containedSystems, float[] emissionBasis, Vector3 positionOffset,
												Vector3 rotationOffset)
				{
					MainObjectTransform = mainObjectTransform;
					ContainedSystems = containedSystems;
					EmissionBasis = emissionBasis;
					PositionOffset = positionOffset;
					RotationOffset = rotationOffset;
				}
			}
		}
	}
}