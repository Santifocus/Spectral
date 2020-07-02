using Spectral.Runtime.Behaviours.Entities;

namespace Spectral.Runtime.DataStorage
{
	public enum ChooseStyle
	{
		Random = 1,
		InOrder = 2,
	}

	public class EntitySettings : SpectralScriptableObject
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
		public int MinParts = 1;

		public DefaultableFloat PartMinimumScale = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityPartMinimumScale);
		public DefaultableFloat ScaleChangePerPart = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityScaleChangePerPart);

		public EntityBodyPartConfiguration EntityHead;
		public ChooseStyle TorsoChooseStyle = ChooseStyle.Random;
		public EntityBodyPartConfiguration[] EntityTorso;
		public EntityBodyPartConfiguration EntityTail;

		//Other
		public DefaultableFloat FoodEatDistance = new DefaultableFloat(() => GameSettings.Current.DefaultEntitySettings.EntityEatDistance);
		public EntityMover OverwritePrefab = default;
		public bool EnableAI = false;
		public EntityAIConfiguration AIConfiguration;
	}
}