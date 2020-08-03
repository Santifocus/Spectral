using Spectral.Sounds;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class SoundTriggerer : MonoBehaviour
	{
		[SerializeField] private Sound[] soundsToTrigger = default;

		public void Trigger()
		{
			foreach (Sound sound in soundsToTrigger)
			{
				MusicController.Instance.PlayPersistantSound(sound);
			}
		}
	}
}