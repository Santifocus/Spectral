using Spectral.Runtime.Behaviours;

namespace Spectral.Runtime.DataStorage
{
	public class EntityBodyPartConfiguration : SpectralScriptableObject
	{
		public EntityFeedbackPlayer PartPrefab;
		public float Lenght = 1;
		public float PartOffset = 0.25f;
		public float AngleLimiter = 50;
		public float Springiness = 10;
		public float SpringDamping = 2;
	}
}