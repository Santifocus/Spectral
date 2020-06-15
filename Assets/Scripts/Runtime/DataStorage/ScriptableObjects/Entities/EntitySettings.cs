using Spectral.Runtime.Factories;
using UnityEngine;

namespace Spectral.Runtime.DataStorage
{
	public enum ChooseStyle
	{
		Random = 1,
		InOrder = 2,
	}

	public class EntitySettings : SpawnableEntity
	{
		//Movement Settings
		public DefaultableFloat MoveSpeed = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityMoveSpeed);
		public DefaultableFloat VelocityDamping = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityVelocityDamping);

		public DefaultableFloat Acceleration = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityAcceleration);
		public DefaultableFloat Deceleration = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityDeceleration);
		public DefaultableFloat AccelerationAngle = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityAccelerationAngle);

		public DefaultableFloat TurnSpeed = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityTurnSpeed);
		public DefaultableFloat TurnAcceleration = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityTurnAcceleration);
		public DefaultableFloat TurnSmoothAngle = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityTurnSmoothAngle);
		public DefaultableFloat TurnSmoothMultiplier = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityTurnSmoothMultiplier);

		//Body Settings
		public int MinParts = 2;
		public int SpawnPartCount = 3;

		public DefaultableFloat PartMinimumScale = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityPartMinimumScale);
		public DefaultableFloat ScaleChangePerPart = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityScaleChangePerPart);

		public EntityBodyPartConfiguration EntityHead;
		public ChooseStyle TorsoChooseStyle = ChooseStyle.Random;
		public EntityBodyPartConfiguration[] EntityTorso;
		public EntityBodyPartConfiguration EntityTail;

		//Other
		public DefaultableFloat FoodEatDistance = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityEatDistance);
		public bool EnableAI = false;
		public EntityAIConfiguration AIConfiguration;

		//Creation
		public override MonoBehaviour Spawn(Vector2 position, int levelPlaneIndex)
		{
			return EntityFactory.CreateEntity(this, SpawnPartCount, position, levelPlaneIndex, Random.Range(0, 360));
		}
	}
}