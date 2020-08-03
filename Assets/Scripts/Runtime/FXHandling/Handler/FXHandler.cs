using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public abstract class FXHandler
	{
		public abstract TimeType UpdateStyle { get; }
		public abstract System.Type FXTargetType { get; }
		public virtual void OnHandlerInitiated() { }
		public abstract FXInstance InitialiseFX(FXObject baseData, FXInstanceData instanceData);
		public abstract void HandleFX(float timeStep);
		protected abstract IEnumerable<FXInstance> GetFXInstances();
		public abstract void Reset();
	}

	public readonly struct FXInstanceData
	{
		public readonly Transform Parent;
		public readonly float Multiplier;
		public readonly float? InitiationDelay;
		public readonly Vector3? CustomPositionOffset;
		public readonly Vector3? CustomRotationOffset;
		public readonly Vector3? CustomScale;

		public FXInstanceData(Transform parent, float multiplier, float? initiationDelay, Vector3? customPositionOffset, Vector3? customRotationOffset,
							Vector3? customScale)
		{
			Parent = parent;
			Multiplier = multiplier;
			InitiationDelay = initiationDelay;
			CustomPositionOffset = customPositionOffset;
			CustomRotationOffset = customRotationOffset;
			CustomScale = customScale;
		}
	}
}