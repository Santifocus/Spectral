using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaiviors
{
    public class SpectralMonoBehavior : MonoBehaviour
    {
        public T GetOrAddComponent<T>() where T : Component => GetComponent<T>() ?? gameObject.AddComponent<T>();
    }
}