using Spectral.Runtime.Behaviours;
using Spectral.Runtime.Behaviours.Entities;
using UnityEngine;

namespace Spectral.Runtime.Factories
{
	public static partial class EntityFactory
	{
		private const float FOOD_OBJECT_BODY_DRAG = 0.8f;
		private const float FOOD_OBJECT_BODY_ANGULAR_DRAG = FOOD_OBJECT_BODY_DRAG / 1.5f;
		private const float FOOD_OBJECT_START_VELOCITY_MUL_MIN = 0.5f;
		private const float FOOD_OBJECT_START_VELOCITY_MUL_MAX = 1.75f;
		private const float FOOD_OBJECT_SPIN = 90;

		public static void TearDownEntity(EntityMover target)
		{
			int bodyParts = GetTotalEntityBodyPartCount(target);
			FoodSpawner targetFoodSpawner = LevelLoader.GameLevelPlanes[target.PlaneLevelIndex ?? LevelLoader.PlayerLevelIndex].CoreObject.AffiliatedFoodSpawner;
			for (int i = 0; i < bodyParts; i++)
			{
				RebuildAsFoodObject(GetBodyPartFromIndex(target, i), target.CurrentVelocity, targetFoodSpawner);
			}

			Object.Destroy(target.gameObject);
		}

		private static void RebuildAsFoodObject(EntityBodyPart bodyPart, Vector2 baseVelocity, FoodSpawner targetFoodSpawner)
		{
			//Setup the Transforms
			Transform foodObjectCore = new GameObject("Body Part Food Object").transform;
			Transform partModel = bodyPart.Model;
			foodObjectCore.SetParent(targetFoodSpawner.TargetPlane.TargetStorage.FoodObjectStorage);
			partModel.SetParent(foodObjectCore);

			//Setup the foodObject
			FoodObject targetFoodObject = foodObjectCore.gameObject.AddComponent<FoodObject>();
			targetFoodObject.Initialise(targetFoodSpawner);
			targetFoodObject.Setup(partModel.position);
			targetFoodObject.FinishExpansion(true);
			partModel.localPosition = Vector3.zero;

			//Setup the rigidbody
			Rigidbody targetRigidbody = foodObjectCore.gameObject.AddComponent<Rigidbody>();
			targetRigidbody.useGravity = false;
			targetRigidbody.velocity = (baseVelocity.XZtoXYZ() + bodyPart.Body.velocity) * Random.Range(FOOD_OBJECT_START_VELOCITY_MUL_MIN, FOOD_OBJECT_START_VELOCITY_MUL_MAX);
			targetRigidbody.angularVelocity = bodyPart.Body.angularVelocity + new Vector3(0, Random.Range(-FOOD_OBJECT_SPIN, FOOD_OBJECT_SPIN) * Mathf.Deg2Rad, 0);
			targetRigidbody.drag = FOOD_OBJECT_BODY_DRAG;
			targetRigidbody.angularDrag = FOOD_OBJECT_BODY_ANGULAR_DRAG;
		}
	}
}