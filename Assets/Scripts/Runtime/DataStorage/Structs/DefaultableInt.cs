using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
{
	[System.Serializable]
	public struct DefaultableInt
	{
		public int RawValue;
		public bool UseDefault;
		public int DefaultOffset;
		public DefaultableInt(int Value, bool UseDefault, int DefaultOffset)
		{
			RawValue = Value;
			this.UseDefault = UseDefault;
			this.DefaultOffset = DefaultOffset;
		}
		public DefaultableInt(int? Value)
		{
			RawValue = Value ?? default;
			UseDefault = !Value.HasValue;
			DefaultOffset = default;
		}
		public int GetDefaultedValue(int valuesDefault)
		{
			return UseDefault ? (valuesDefault + DefaultOffset) : RawValue;
		}
	}
}