using UnityEngine;

namespace Spectral.Runtime.Interfaces
{
	public interface IPoolableObject<T> where T : Component, IPoolableObject<T>
	{
		ObjectPool<T> SelfPool { get; set; }
		void ReturnToPool();
	}
}