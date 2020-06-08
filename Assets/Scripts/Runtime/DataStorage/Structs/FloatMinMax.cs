using UnityEngine;

namespace Spectral.Runtime.DataStorage
{
	[System.Serializable]
	public struct FloatMinMax
	{
		public float Min;
		public float Max;

		public FloatMinMax(float min, float max)
		{
			Min = min;
			Max = max;
		}

		public float Random => UnityEngine.Random.Range(Min, Max);

		public float Clamped(float value)
		{
			return Mathf.Clamp(value, Min, Max);
		}

		public bool WithIn(float value)
		{
			return (value >= Min) && (value <= Max);
		}
	}
}