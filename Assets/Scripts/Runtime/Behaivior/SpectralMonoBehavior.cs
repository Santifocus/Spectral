using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class SpectralMonoBehavior : MonoBehaviour
	{
		public T GetOrAddComponent<T>() where T : Component
		{
			return GetComponent<T>() ?? gameObject.AddComponent<T>();
		}
	}
}