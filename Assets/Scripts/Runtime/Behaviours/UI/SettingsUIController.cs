using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spectral.Runtime;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.Behaviours.UI;
using Spectral.Runtime.DataStorage;
using Spectral.Runtime.DataStorage.FX;
using Spectral.Runtime.DataStorage.Logic;
using Spectral.Runtime.Factories;
using Spectral.Runtime.FX.Handling;
using Spectral.Runtime.Interfaces;
using UnityEngine.UI;


namespace Spectral.Runtime.Behaviours.UI
{
	public class SettingsUIController : MonoBehaviour
	{
		[Header("Music")]
		[SerializeField] private Slider musicVolumeSlider = default;
		[SerializeField] private GameObject musicVolumeToggleOn = default;
		[SerializeField] private GameObject musicVolumeToggleOff = default;
		
		[Header("Sound")]
		[SerializeField] private Slider soundVolumeSlider = default;
		[SerializeField] private GameObject soundVolumeToggleOn = default;
		[SerializeField] private GameObject soundVolumeToggleOff = default;

		public event Action SettingsMenuWantsToClose;
		
		public void Initialise()
		{
			musicVolumeSlider.value = PersistentSettingsManager.CurrentSettings.MusicVolumeScale;
			UpdateToggle(musicVolumeToggleOn, musicVolumeToggleOff, PersistentSettingsManager.CurrentSettings.MusicEnabled);
			soundVolumeSlider.value = PersistentSettingsManager.CurrentSettings.SoundVolumeScale;
			UpdateToggle(soundVolumeToggleOn, soundVolumeToggleOff, PersistentSettingsManager.CurrentSettings.SoundEnabled);
		}

		public void ApplySettings()
		{
			PersistentSettingsManager.SaveOrCreateSettings();
			SettingsMenuWantsToClose?.Invoke();
		}

		public void Cancel()
		{
			PersistentSettingsManager.CurrentSettings.ResetCurrentSettings();
			SettingsMenuWantsToClose?.Invoke();
		}

		public void MusicVolumeSliderChanged()
		{
			PersistentSettingsManager.CurrentSettings.ActiveMusicVolumeScale = musicVolumeSlider.value;
		}
		
		public void SoundVolumeSliderChanged()
		{
			PersistentSettingsManager.CurrentSettings.ActiveSoundVolumeScale = soundVolumeSlider.value;
		}

		public void ToggleMusicVolume()
		{
			PersistentSettingsManager.CurrentSettings.ActiveMusicEnabled = !PersistentSettingsManager.CurrentSettings.ActiveMusicEnabled;
			UpdateToggle(musicVolumeToggleOn, musicVolumeToggleOff, PersistentSettingsManager.CurrentSettings.ActiveMusicEnabled);
		}
		
		public void ToggleSoundVolume()
		{
			PersistentSettingsManager.CurrentSettings.ActiveSoundEnabled = !PersistentSettingsManager.CurrentSettings.ActiveSoundEnabled;
			UpdateToggle(soundVolumeToggleOn, soundVolumeToggleOff, PersistentSettingsManager.CurrentSettings.ActiveSoundEnabled);
		}

		private void UpdateToggle(GameObject onState, GameObject offState, bool currentState)
		{
			onState.SetActive(currentState);
			offState.SetActive(!currentState);
		}
	}
}