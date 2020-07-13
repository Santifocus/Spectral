using Spectral.Runtime.Factories;
using UnityEngine;

namespace Spectral.Runtime.DataStorage
{
	public class ObstacleSettings : SpawnableEntity
	{
		//General
		public GameObject Model;

		//Damage
		public float DamageRange = 1;
		public int DamageAmount = 1;
		public float DamageImpactForce = 6;
		public float DamageCooldown = 1;

		//Movement
		public float MoveSpeed = 3;
		public bool BounceOnDealDamage = true;

		//Creation
		public override MonoBehaviour Spawn(Vector2 position, int levelPlaneIndex)
		{
			return ObstacleFactory.CreateObstacle(this, position, levelPlaneIndex);
		}
	}
}