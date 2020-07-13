using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Factories
{
	public static class ObstacleFactory
	{
		public static Obstacle CreateObstacle(ObstacleSettings settings, Vector2 position, int levelIndex)
		{
			//Setup core object
			Transform obstacleCoreObject = new GameObject("Obstacle Entity").transform;
			obstacleCoreObject.SetParent(LevelLoader.GameLevelPlanes[levelIndex].CoreObject.TargetStorage.EntityStorage);
			obstacleCoreObject.localPosition = position.XZtoXYZ();

			//Setup the Obstacle behaviour
			Obstacle obstacle = obstacleCoreObject.gameObject.AddComponent<Obstacle>();
			obstacle.Initialise(settings);

			return obstacle;
		}
	}
}