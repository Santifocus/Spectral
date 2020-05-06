using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
{
	public class EntityBodyPartSettings : SpectralScriptableObject
	{
		public GameObject PartPrefab;
		public float Lenght = 1;
		public float PartOffset = 0.25f;
		public float AngleLimiter = 50;
		public float Springiness = 10;
		public float SpringDamping = 2;
	}
}