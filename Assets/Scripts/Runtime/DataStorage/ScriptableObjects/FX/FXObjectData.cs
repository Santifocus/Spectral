using UnityEngine;

namespace Spectral.Runtime.DataStorage.FX
{
	[System.Flags]
	public enum FXDataOverwrite
	{
		InitiationDelay = 1 << 0,
		Position = 1        << 1,
		Rotation = 1        << 2,
		Scale = 1           << 3,
	}

	[System.Serializable]
	public class FXObjectData
	{
		public FXObject BaseFX;
		public FXDataOverwrite EnabledOverwrites = 0;
		public FloatMinMax MultiplierClamps = new FloatMinMax(1, 1);

		public float InitiationDelay = 0;
		public Vector3 PositionOffset = Vector3.zero;
		public Vector3 RotationOffset = Vector3.zero;
		public Vector3 NewScale = Vector3.one;
	}
}