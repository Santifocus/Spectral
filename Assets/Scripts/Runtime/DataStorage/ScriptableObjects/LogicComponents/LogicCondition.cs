namespace Spectral.Runtime.DataStorage.Logic
{
	public abstract class LogicCondition : SpectralScriptableObject
	{
		public bool Inverse;
		public bool True => InternalTrue != Inverse;
		protected abstract bool InternalTrue { get; }

		public static implicit operator bool(LogicCondition self)
		{
			return !(self is null) && self.True;
		}
	}
}