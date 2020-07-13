using System;
using UnityEngine;
using UnityEngine.UI;

namespace Spectral.Runtime.Behaviours.UI
{
	public class SettingsUIController : MonoBehaviour
	{
		[Header("Music")] [SerializeField] private Slider musicVolumeSlider = default;
		[SerializeField] private GameObject musicVolumeToggleOn = default;
		[SerializeField] private GameObject musicVolumeToggleOff = default;

		[Header("Sound")] [SerializeField] private Slider soundVolumeSlider = default;
		[SerializeField] private GameObject soundVolumeToggleOn = default;
		[SerializeField] private GameObject soundVolumeToggleOff = default;

		public event Action SettingsMenuWantsToClose;

		public void Initialise()
		{
			musicVolumeSlider.value = PersistentDataManager.CurrentSettings.MusicVolumeScale;
			UpdateToggle(musicVolumeToggleOn, musicVolumeToggleOff, PersistentDataManager.CurrentSettings.MusicEnabled);
			soundVolumeSlider.value = PersistentDataManager.CurrentSettings.SoundVolumeScale;
			UpdateToggle(soundVolumeToggleOn, soundVolumeToggleOff, PersistentDataManager.CurrentSettings.SoundEnabled);
		}

		public void ApplySettings()
		{
			PersistentDataManager.SaveOrCreateSettings();
			SettingsMenuWantsToClose?.Invoke();
		}

		public void Cancel()
		{
			PersistentDataManager.CurrentSettings.ResetCurrentSettings();
			SettingsMenuWantsToClose?.Invoke();
		}

		public void MusicVolumeSliderChanged()
		{
			PersistentDataManager.CurrentSettings.ActiveMusicVolumeScale = musicVolumeSlider.value;
		}

		public void SoundVolumeSliderChanged()
		{
			PersistentDataManager.CurrentSettings.ActiveSoundVolumeScale = soundVolumeSlider.value;
		}

		public void ToggleMusicVolume()
		{
			PersistentDataManager.CurrentSettings.ActiveMusicEnabled = !PersistentDataManager.CurrentSettings.ActiveMusicEnabled;
			UpdateToggle(musicVolumeToggleOn, musicVolumeToggleOff, PersistentDataManager.CurrentSettings.ActiveMusicEnabled);
		}

		public void ToggleSoundVolume()
		{
			PersistentDataManager.CurrentSettings.ActiveSoundEnabled = !PersistentDataManager.CurrentSettings.ActiveSoundEnabled;
			UpdateToggle(soundVolumeToggleOn, soundVolumeToggleOff, PersistentDataManager.CurrentSettings.ActiveSoundEnabled);
		}

		private void UpdateToggle(GameObject onState, GameObject offState, bool currentState)
		{
			onState.SetActive(currentState);
			offState.SetActive(!currentState);
		}
	}
}