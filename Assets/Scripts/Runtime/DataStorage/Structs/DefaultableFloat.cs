using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
{
	[System.Serializable]
	public struct DefaultableFloat
	{
		public float RawValue;
		public bool UseDefault;
		public float DefaultOffset;
		public DefaultableFloat(float Value, bool UseDefault, float DefaultOffset)
		{
			RawValue = Value;
			this.UseDefault = UseDefault;
			this.DefaultOffset = DefaultOffset;
		}
		public DefaultableFloat(float? Value)
		{
			RawValue = Value ?? default;
			UseDefault = !Value.HasValue;
			DefaultOffset = default;
		}
		public float GetDefaultedValue(float valuesDefault)
		{
			return UseDefault ? (valuesDefault + DefaultOffset) : RawValue;
		}
	}
}