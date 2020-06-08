namespace Spectral.Runtime.DataStorage
{
	[System.Serializable]
	public struct DefaultableFloat
	{
		public float NonDefaultValue;
		public bool UseDefault;
		public float DefaultOffset;
		public System.Func<float> DefaultGetter;

		public DefaultableFloat(float nonDefaultValue, bool useDefault, float defaultOffset, System.Func<float> defaultGetter)
		{
			NonDefaultValue = nonDefaultValue;
			UseDefault = useDefault;
			DefaultOffset = defaultOffset;
			DefaultGetter = defaultGetter;
		}

		public DefaultableFloat(System.Func<float> defaultGetter)
		{
			NonDefaultValue = default;
			UseDefault = true;
			DefaultOffset = default;
			DefaultGetter = defaultGetter;
		}

		private float GetDefaultedValue()
		{
			return UseDefault ? DefaultGetter.Invoke() + DefaultOffset : NonDefaultValue;
		}

		public static implicit operator float(DefaultableFloat target)
		{
			return target.GetDefaultedValue();
		}
	}
}