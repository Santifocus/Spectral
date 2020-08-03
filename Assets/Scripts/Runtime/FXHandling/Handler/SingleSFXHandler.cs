using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using Spectral.Sounds;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class SingleSFXHandler : FXHandler
	{
		public override TimeType UpdateStyle => TimeType.FixedDeltaTime;
		public override System.Type FXTargetType => typeof(SingleSFX);
		private readonly List<SingleSFXInstance> effectInstances = new List<SingleSFXInstance>();

		public override FXInstance InitialiseFX(FXObject baseData, FXInstanceData instanceData)
		{
			SingleSFXInstance newInstance = new SingleSFXInstance(baseData, instanceData);
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
			foreach (SingleSFXInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset() { }

		private class SingleSFXInstance : FXInstance<SingleSFX>
		{
			private SingleSFXInstanceData singleSFXInstanceData;
			public SingleSFXInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			protected override void OnInitiate()
			{
				base.OnInitiate();
				Vector3 positionOffset = InstanceData.CustomPositionOffset ?? FXData.OffsetToTarget;
				SoundPlayer soundPlayer = new GameObject(FXData.TargetSound.soundName + " Sound Player").AddComponent<SoundPlayer>();
				soundPlayer.transform.SetParent(LevelLoader.CoreStorage.ParticleStorage);
				soundPlayer.Setup(FXData.TargetSound);
				soundPlayer.Play();
				singleSFXInstanceData = new SingleSFXInstanceData(soundPlayer.transform, soundPlayer, positionOffset);
			}

			protected override void InternalUpdate(float timeStep)
			{
				UpdateTransform();
				singleSFXInstanceData.SoundPlayer.FadePoint = GetCurrentMultiplier();
				if (singleSFXInstanceData.SoundPlayer.FullyStopped)
				{
					RequestFinishFX();
				}
			}

			private void UpdateTransform()
			{
				if (!FXData.FollowTarget || !InstanceData.Parent)
				{
					return;
				}

				//Update position
				singleSFXInstanceData.MainObjectTransform.position = InstanceData.Parent.position + InstanceData.Parent.TransformVector(singleSFXInstanceData.PositionOffset);
			}

			protected override void OnDestroy()
			{
				Object.Destroy(singleSFXInstanceData.MainObjectTransform.gameObject);
			}

			private readonly struct SingleSFXInstanceData
			{
				public readonly Transform MainObjectTransform;
				public readonly SoundPlayer SoundPlayer;
				public readonly Vector3 PositionOffset;

				public SingleSFXInstanceData(Transform mainObjectTransform, SoundPlayer soundPlayer, Vector3 positionOffset)
				{
					MainObjectTransform = mainObjectTransform;
					SoundPlayer = soundPlayer;
					PositionOffset = positionOffset;
				}
			}
		}
	}
}