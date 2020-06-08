using UnityEngine;

namespace Spectral.Runtime
{
	public class Storage : MonoBehaviour
	{
		private static Storage Instance { get; set; }

		public static Transform EntityStorage => Instance.entityStorage;
		public static Transform ParticleStorage => Instance.particleStorage;
		public static Transform FoodObjectStorage => Instance.foodObjectStorage;

		[SerializeField] private Transform entityStorage = default;
		[SerializeField] private Transform particleStorage = default;
		[SerializeField] private Transform foodObjectStorage = default;

		private void Start()
		{
			Instance = this;
		}
	}
}