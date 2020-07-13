using Spectral.Runtime.Factories;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class LevelPlane : MonoBehaviour
	{
		public int LevelPlaneIndex { get; private set; }
		public LevelSettings PlaneSettings { get; private set; }
		public FoodSpawner AffiliatedFoodSpawner { get; private set; }
		public Storage TargetStorage { get; private set; }

		public TransitionGate downTransitionGate { get; private set; }
		public TransitionGate upTransitionGate { get; private set; }

		public void Initialise(int levelPlaneIndex)
		{
			LevelPlaneIndex = levelPlaneIndex;
			PlaneSettings = LevelLoader.GameLevelPlanes[LevelPlaneIndex].PlaneSettings;

			//Planes Storage
			TargetStorage = new Storage(transform);

			//Transition Gates
			CreateTransitionGates();

			//Food Spawner
			AffiliatedFoodSpawner = gameObject.AddComponent<FoodSpawner>();
			AffiliatedFoodSpawner.Initialise(this);
		}

		private void CreateTransitionGates()
		{
			if (LevelPlaneIndex > 0)
			{
				upTransitionGate = TransitionGateFactory.CreateTransitionGate(-1, LevelPlaneIndex);
			}

			if (LevelPlaneIndex < (LevelLoader.GameLevelPlanes.Length - 1))
			{
				downTransitionGate = TransitionGateFactory.CreateTransitionGate(1, LevelPlaneIndex);
			}
		}

		public TransitionGate GetNearestTransitionGate(Vector2 point, float? maxRange = null)
		{
			TransitionGate target = null;
			float shortestDist = (maxRange * maxRange) ?? Mathf.Infinity;
			if (upTransitionGate)
			{
				float dist = (upTransitionGate.transform.position.XYZtoXZ() - point).sqrMagnitude;
				if (dist < shortestDist)
				{
					shortestDist = dist;
					target = upTransitionGate;
				}
			}

			if (downTransitionGate)
			{
				float dist = (downTransitionGate.transform.position.XYZtoXZ() - point).sqrMagnitude;
				if (dist < shortestDist)
				{
					target = downTransitionGate;
				}
			}

			return target;
		}
	}
}