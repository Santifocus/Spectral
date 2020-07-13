using UnityEngine;

namespace Spectral.Runtime.DataStorage
{
	public abstract class SpawnableEntity : SpectralScriptableObject
	{
		public abstract MonoBehaviour Spawn(Vector2 position, int levelPlaneIndex);
	}
}