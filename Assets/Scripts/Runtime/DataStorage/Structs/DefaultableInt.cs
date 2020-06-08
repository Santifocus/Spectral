namespace Spectral.Runtime.DataStorage
{
	[System.Serializable]
	public struct DefaultableInt
	{
		public int NonDefaultValue;
		public bool UseDefault;
		public int DefaultOffset;
		public System.Func<int> DefaultGetter;

		public DefaultableInt(int nonDefaultValue, bool useDefault, int defaultOffset, System.Func<int> defaultGetter)
		{
			NonDefaultValue = nonDefaultValue;
			UseDefault = useDefault;
			DefaultOffset = defaultOffset;
			DefaultGetter = defaultGetter;
		}

		public DefaultableInt(System.Func<int> defaultGetter)
		{
			NonDefaultValue = default;
			UseDefault = true;
			DefaultOffset = default;
			DefaultGetter = defaultGetter;
		}

		private int GetDefaultedValue()
		{
			return UseDefault ? DefaultGetter.Invoke() + DefaultOffset : NonDefaultValue;
		}

		public static implicit operator int(DefaultableInt target)
		{
			return target.GetDefaultedValue();
		}
	}
}