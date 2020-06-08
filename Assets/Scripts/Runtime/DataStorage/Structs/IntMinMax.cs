using UnityEngine;

namespace Spectral.Runtime.Runtime.DataStorage
{
	[System.Serializable]
	public struct IntMinMax
	{
		public int Min;
		public int Max;


		public IntMinMax(int min, int max)
		{
			Min = min;
			Max = max;
		}

		public int Random => UnityEngine.Random.Range(Min, Max + 1);

		public float Clamped(int value)
		{
			return Mathf.Clamp(value, Min, Max);
		}

		public bool WithIn(int value)
		{
			return (value >= Min) && (value <= Max);
		}
	}
}