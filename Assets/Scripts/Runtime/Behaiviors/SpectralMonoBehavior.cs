using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class SpectralMonoBehavior : MonoBehaviour
	{
		private int? planeLevelIndex;

		public PlaneLevelData AffiliatedLevelPlane => PlaneLevelIndex.HasValue ? LevelLoader.GameLevelPlanes[PlaneLevelIndex.Value].CoreObject : null;

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

		private int? RetrievePlaneLevelIndex()
		{
			Transform parent = transform.parent;
			while (parent)
			{
				PlaneLevelData planeLevelData = parent.GetComponent<PlaneLevelData>();
				if (planeLevelData)
				{
					return planeLevelData.PlaneLevelIndex;
				}

				parent = parent.parent;
			}

			return null;
		}
	}
}