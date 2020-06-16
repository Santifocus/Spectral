using Spectral.Runtime.FX.Handling;

namespace Spectral.Runtime.DataStorage.FX
{
	public abstract class FXObject : SpectralScriptableObject
	{
		public float TimeIn;
		public float TimeOut;
		public FXFinishCondition FinishConditions = new FXFinishCondition();
	}
}