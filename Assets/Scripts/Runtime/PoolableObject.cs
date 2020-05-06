using System.Collections.Generic;
using UnityEngine;

namespace Spectral
{
	public interface IPoolableObject<T> where T : MonoBehaviour, IPoolableObject<T>
	{
		PoolableObject<T> SelfPool { get; set; }
		void ReturnToPool();
	}
	public class PoolableObject<T> where T : MonoBehaviour, IPoolableObject<T>
	{
		private int poolSize;
		public int PoolSize { get => poolSize; set => IncreasePoolsize(value); }
		public List<T> ActiveObjects { get; }

		private readonly T objectToPool;
		private readonly Transform poolParent;
		private readonly bool reAdjustPoolsize;
		private readonly Queue<T> inActiveObjects;

		public PoolableObject(int PoolSize, bool ReAdjustPoolsize, T ObjectToPool, Transform PoolParent)
		{
			objectToPool = ObjectToPool;
			poolParent = PoolParent;
			reAdjustPoolsize = ReAdjustPoolsize;

			ActiveObjects = new List<T>((int)PoolSize);
			inActiveObjects = new Queue<T>((int)PoolSize);

			IncreasePoolsize(PoolSize);
		}

		private void IncreasePoolsize(int newPoolSize)
		{
			int dif = newPoolSize - poolSize;
			if (dif <= 0)
			{
				return;
			}

			poolSize = newPoolSize;
			ActiveObjects.Capacity = (int)newPoolSize;

			for (uint i = 0; i < dif; i++)
			{
				T newObject = Object.Instantiate(objectToPool, poolParent);
				newObject.gameObject.SetActive(false);
				newObject.SelfPool = this;

				inActiveObjects.Enqueue(newObject);
			}
		}

		public T GetPoolObject()
		{
			if (inActiveObjects.Count == 0)
			{
				if (reAdjustPoolsize)
				{
					IncreasePoolsize(poolSize + 1);
				}
				else
				{
					return null;
				}
			}

			T ret = inActiveObjects.Dequeue();
			ActiveObjects.Add(ret);
			ret.gameObject.SetActive(true);

			return ret;
		}
		public IEnumerable<T> GetMultipleObjects(int amount)
		{
			if (inActiveObjects.Count < amount)
			{
				if (reAdjustPoolsize)
				{
					IncreasePoolsize(poolSize + (amount - inActiveObjects.Count));
				}
				else
				{
					yield break;
				}
			}

			for (int i = 0; i < amount; i++)
			{
				T obj = inActiveObjects.Dequeue();
				obj.gameObject.SetActive(true);
				yield return obj;
			}
		}
		public void ReturnPoolObject(T toQueue)
		{
			ActiveObjects.Remove(toQueue);
			toQueue.gameObject.SetActive(false);
			inActiveObjects.Enqueue(toQueue);
		}
	}
}