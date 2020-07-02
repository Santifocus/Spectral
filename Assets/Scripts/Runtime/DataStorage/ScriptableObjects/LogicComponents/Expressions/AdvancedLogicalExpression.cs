namespace Spectral.Runtime.DataStorage.Logic
{
	public class AdvancedLogicalExpression : LogicCondition
	{
		public int MinimumTrue;
		public int MaximumTrue;
		public LogicCondition[] Conditions = new LogicCondition[0];

		protected override bool InternalTrue
		{
			get
			{
				int trueCount = 0;
				for (int i = 0; i < Conditions.Length; i++)
				{
					if (Conditions[i].True)
					{
						trueCount++;
						if (trueCount > MaximumTrue)
						{
							break;
						}
					}
				}

				return ((trueCount >= MinimumTrue) && (trueCount <= MaximumTrue)) != Inverse;
			}
		}
	}
}