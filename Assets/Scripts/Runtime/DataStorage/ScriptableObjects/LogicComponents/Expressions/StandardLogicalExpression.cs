namespace Spectral.Runtime.DataStorage.Logic
{
	public class StandardLogicalExpression : LogicCondition
	{
		public LogicCondition[] LogicalComponents = new LogicCondition[0];

		protected override bool InternalTrue
		{
			get
			{
				for (int i = 0; i < LogicalComponents.Length; i++)
				{
					if (LogicalComponents[i].True)
					{
						return true;
					}
				}

				return false;
			}
		}
	}
}