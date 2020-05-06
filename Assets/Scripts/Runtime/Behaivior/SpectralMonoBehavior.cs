using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaviours
{
	public class SpectralMonoBehavior : MonoBehaviour
	{
		public T GetOrAddComponent<T>() where T : Component
		{
			return GetComponent<T>() ?? gameObject.AddComponent<T>();
		}
	}
}