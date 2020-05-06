using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spectral.DataStorage;

namespace Spectral.Behaviours
{
	public class EntityBodyPart : MonoBehaviour
	{
		public EntityBodyPartSettings Data { get; set; }
		public float CalculatedLenght { get; set; }
	}
}