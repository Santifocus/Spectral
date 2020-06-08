using Spectral.Runtime.DataStorage.Logic;

namespace Spectral.Runtime.FX.Handling
{
	[System.Serializable]
	public class FXFinishCondition
	{
		public bool OnParentDeath;
		public bool OnTimeout;
		public float TimeStay = 1;
		public bool OnConditionMet;
		public LogicCondition Condition;

		public bool ConditionMet()
		{
			return Condition.True;
		}
	}
}