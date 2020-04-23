using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Behaiviors
{
	public class SmoothCamera : SpectralMonoBehavior
	{
		[SerializeField] private Transform followTarget = default;
		[SerializeField] private Vector3 followOffset = new Vector3(0,10,0);
		[SerializeField] private bool startAttached = true;

		[Space(6)]
		[SerializeField] private float followSpeed = 0.1f;
		[SerializeField] private float speedAcceleration = 5;

		private Vector3 currentVelocity;

		private void Start()
		{
			if (startAttached && followTarget)
			{
				transform.position = followTarget.position + followOffset;
			}
		}
		private void FixedUpdate()
		{
			Vector3 difference = followTarget ? ((followTarget.transform.position + followOffset) - transform.position) : Vector3.zero;

			currentVelocity = Vector3.Lerp(currentVelocity, difference * followSpeed, speedAcceleration * Time.fixedDeltaTime);
			transform.position += currentVelocity;
		}
	}
}