using UnityEngine;

namespace Spectral.DataStorage
{
	[System.Serializable]
	public struct IntMinMax
	{
		public int Min;
		public int Max;

		public int Rand => Random.Range(Min, Max + 1);
		public IntMinMax(int Min, int Max)
		{
			this.Min = Min;
			this.Max = Max;
		}
		public bool WithIn(int value)
		{
			return value >= Min && value <= Max;
		}
	}
}