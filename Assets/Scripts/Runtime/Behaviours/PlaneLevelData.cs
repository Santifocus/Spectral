using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class PlaneLevelData : MonoBehaviour
	{
		public int PlaneLevelIndex { get; private set; }
		public LevelSettings PlaneSettings => LevelLoader.GameLevelPlanes[PlaneLevelIndex].PlaneSettings;
		public FoodSpawner AffiliatedFoodSpawner { get; private set; }
		public Storage TargetStorage { get; private set; }

		public void Initiate(int levelPlaneIndex)
		{
			PlaneLevelIndex = levelPlaneIndex;

			//Planes Storage
			TargetStorage = new Storage(transform);

			//Food Spawner
			AffiliatedFoodSpawner = gameObject.AddComponent<FoodSpawner>();
			AffiliatedFoodSpawner.Initiate(this);
		}
	}
}