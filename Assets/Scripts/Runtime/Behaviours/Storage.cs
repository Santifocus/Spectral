using UnityEngine;

namespace Spectral.Runtime
{
	public class Storage
	{
		public readonly Transform Main;
		public readonly Transform EntityStorage;
		public readonly Transform ParticleStorage;
		public readonly Transform FoodObjectStorage;

		public Storage(Transform parent)
		{
			Main = new GameObject("Storage Main").transform;
			Main.SetParent(parent);

			//EntityStorage
			EntityStorage = new GameObject("EntityStorage").transform;
			EntityStorage.SetParent(Main);

			//ParticleStorage
			ParticleStorage = new GameObject("ParticleStorage").transform;
			ParticleStorage.SetParent(Main);

			//FoodObjectStorage
			FoodObjectStorage = new GameObject("FoodObjectStorage").transform;
			FoodObjectStorage.SetParent(Main);
		}
	}
}