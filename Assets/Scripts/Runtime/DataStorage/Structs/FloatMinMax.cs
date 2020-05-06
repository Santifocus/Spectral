using UnityEngine;

namespace Spectral.DataStorage
{
	[System.Serializable]
	public struct FloatMinMax
	{
		public float Min;
		public float Max;

		public FloatMinMax(float Min, float Max)
		{
			this.Min = Min;
			this.Max = Max;
		}

		public float Rand => Random.Range(Min, Max);
		public bool WithIn(float value)
		{
			return value >= Min && value <= Max;
		}
	}
}