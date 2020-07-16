using Spectral.Runtime;
using Spectral.Runtime.Behaviours.UI;
using UnityEngine;

namespace Spectral.Sounds
{
	public class SoundPlayer : MonoBehaviour
	{
		private Sound baseData;
		private AudioSource source;
		public bool Exists => source;

		public float FadePoint
		{
			get => fadePoint;
			set
			{
				fadePoint = value;
				source.volume = baseData.volume * volumeScale * value;
			}
		}

		private float fadePoint;
		public bool FullyStopped => !isPaused && !source.isPlaying;
		private bool isPaused;

		private float volumeScale => baseData.soundCategory == SoundCategory.Music
										? (PersistentDataManager.CurrentSettings.ActiveMusicEnabled ? PersistentDataManager.CurrentSettings.ActiveMusicVolumeScale : 0)
										: (PersistentDataManager.CurrentSettings.ActiveSoundEnabled ? PersistentDataManager.CurrentSettings.ActiveSoundVolumeScale : 0);

		public void Setup(Sound baseData)
		{
			source = gameObject.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.Stop();
			RetrieveData(baseData);
			FadePoint = 1;
			SettingsUIController.VolumeScalesChangedEvent += UpdateVolumeScale;
		}

		private void RetrieveData(Sound baseData)
		{
			this.baseData = baseData;
			source.clip = baseData.clip;
			source.pitch = baseData.pitch;
			source.loop = baseData.Loop;
			source.spatialBlend = baseData.spatialBlend;
			source.volume = baseData.volume * volumeScale;
			source.priority = baseData.priority;
			source.outputAudioMixerGroup = baseData.audioGroup;
		}

		private void UpdateVolumeScale()
		{
			//Just update with the set volumes
			FadePoint = FadePoint;
		}

		private void Update()
		{
			UpdateScale();
		}

		private void UpdateScale()
		{
			if (baseData.scaleStyle == SoundTimeScale.Unscaled)
			{
				return;
			}

			if (baseData.scaleStyle == SoundTimeScale.Scaled)
			{
				if (StaticData.GameIsPaused == source.isPlaying)
				{
					if (StaticData.GameIsPaused)
					{
						isPaused = true;
						source.Pause();
					}
					else
					{
						isPaused = false;
						source.UnPause();
					}
				}

				source.pitch = baseData.pitch * Time.timeScale;
			}
			else if (StaticData.GameIsPaused == source.isPlaying)
			{
				if (StaticData.GameIsPaused)
				{
					isPaused = true;
					source.Pause();
				}
				else
				{
					isPaused = false;
					source.UnPause();
				}
			}
		}

		public void Play()
		{
			if (source.isPlaying)
			{
				source.Stop();
			}

			isPaused = false;
			source.Play();
		}

		public void Stop()
		{
			if (source.isPlaying)
			{
				source.Stop();
			}

			isPaused = false;
		}

		private void OnDestroy()
		{
			SettingsUIController.VolumeScalesChangedEvent -= UpdateVolumeScale;
		}
	}
}