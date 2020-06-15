using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class PlaneLevelData : MonoBehaviour
	{
		public int PlaneLevelIndex { get; private set; }
		public LevelSettings PlaneSettings { get; private set; }
		public FoodSpawner AffiliatedFoodSpawner { get; private set; }
		public Storage TargetStorage { get; private set; }

		public void Initialise(int levelPlaneIndex)
		{
			PlaneLevelIndex = levelPlaneIndex;
			PlaneSettings = LevelLoader.GameLevelPlanes[PlaneLevelIndex].PlaneSettings;

			//Planes Storage
			TargetStorage = new Storage(transform);

			//Food Spawner
			AffiliatedFoodSpawner = gameObject.AddComponent<FoodSpawner>();
			AffiliatedFoodSpawner.Initialise(this);
		}
	}
}