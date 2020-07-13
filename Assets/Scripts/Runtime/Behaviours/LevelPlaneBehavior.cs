using Spectral.Runtime.Behaviours.Entities;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class LevelPlaneBehavior : MonoBehaviour
	{
		private int? planeLevelIndex;

		public LevelPlane AffiliatedLevelPlane => PlaneLevelIndex.HasValue ? LevelLoader.GameLevelPlanes[PlaneLevelIndex.Value].CoreObject : null;

		public int? PlaneLevelIndex
		{
			get
			{
				if (planeLevelIndex.HasValue)
				{
					return planeLevelIndex;
				}
				else
				{
					return planeLevelIndex = RetrievePlaneLevelIndex();
				}
			}
		}

		public T GetOrAddComponent<T>() where T : Component
		{
			return GetComponent<T>() ?? gameObject.AddComponent<T>();
		}

		public bool SamePlaneAsPlayerInstance()
		{
			return PlayerMover.Existent && !LevelLoader.Transitioning && (PlaneLevelIndex == LevelLoader.PlayerLevelIndex);
		}

		private int? RetrievePlaneLevelIndex()
		{
			Transform parent = transform.parent;
			while (parent)
			{
				LevelPlane planeLevelData = parent.GetComponent<LevelPlane>();
				if (planeLevelData)
				{
					return planeLevelData.LevelPlaneIndex;
				}

				parent = parent.parent;
			}

			return null;
		}
	}
}