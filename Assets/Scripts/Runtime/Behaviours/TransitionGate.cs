using System;
using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.Factories;

namespace Spectral.Runtime.Behaviours
{
	public class TransitionGate : LevelPlaneBehavior
	{
		public static event Action<int, bool> PlayerWantsToStartLevelTransition;

		private int transitionDirection;
		private bool wasActivatedBefore;

		public void Initialise(int transitionDirection)
		{
			this.transitionDirection = transitionDirection;
			if (PlaneLevelIndex < LevelLoader.PlayerLevelIndex)
			{
				wasActivatedBefore = true;
			}
		}

		public void TryActivate()
		{
			if (transitionDirection == -1)
			{
				Activate();
			}
			else
			{
				if (CanBeActivated())
				{
					Activate();
				}
			}
		}

		public bool CanBeActivated()
		{
			return SamePlaneAsPlayerInstance() &&
					(wasActivatedBefore || (EntityFactory.GetEntitySize(PlayerMover.Instance) >= AffiliatedLevelPlane.PlaneSettings.RequiredPlayerSizeToTransition));
		}

		private void Activate()
		{
			PlayerWantsToStartLevelTransition?.Invoke(transitionDirection, wasActivatedBefore);
			wasActivatedBefore = true;
		}
	}
}