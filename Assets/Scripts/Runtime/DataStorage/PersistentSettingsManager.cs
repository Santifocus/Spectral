using System.IO;
using UnityEngine;

namespace Spectral.Runtime
{
	public static class PersistentSettingsManager
	{
		private const int EXPECTED_FILE_LENGTH = 10;
		private const string SETTINGS_FILE_NAME = "SpectralSettings";
		private const string SETTINGS_FILE_EXTENSION = "data";
		private const string DOT = ".";
		private const string SAVE_FOLDER_PATH = "Data";

		public static readonly SpectralSettings CurrentSettings = new SpectralSettings();

		public static void ReadOrCreate()
		{
			if (!ReadSettings())
			{
				SaveOrCreateSettings();
			}
		}

		public static void ResetSettings()
		{
			CurrentSettings.ResetSettings();
			SaveOrCreateSettings();
		}

		public static void SaveOrCreateSettings()
		{
			string folderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH);
			string savePath = Path.Combine(folderPath, SETTINGS_FILE_NAME + DOT + SETTINGS_FILE_EXTENSION);
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

#if SPECTRAL_DEBUG
			Debug.Log($"Saving at: {savePath}");
#endif
			using (FileStream fileStream = File.Open(savePath, FileMode.Create))
			{
				BinaryWriter writer = new BinaryWriter(fileStream);
				CurrentSettings.ApplyCurrent();
				writer.Write(CurrentSettings.MusicVolumeScale);
				writer.Write(CurrentSettings.SoundVolumeScale);
				writer.Write(CurrentSettings.MusicEnabled);
				writer.Write(CurrentSettings.SoundEnabled);
			}
		}

		private static bool ReadSettings()
		{
			string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH, SETTINGS_FILE_NAME + DOT + SETTINGS_FILE_EXTENSION);
			if (!File.Exists(savePath))
			{
				return false;
			}

#if SPECTRAL_DEBUG
			Debug.Log($"Reading at: {savePath}");
#endif
			using (FileStream fileStream = File.Open(savePath, FileMode.Open))
			{
				if (fileStream.Length != EXPECTED_FILE_LENGTH)
				{
					fileStream.Close();

					return false;
				}

				BinaryReader reader = new BinaryReader(fileStream);
				CurrentSettings.MusicVolumeScale = reader.ReadSingle();
				CurrentSettings.SoundVolumeScale = reader.ReadSingle();
				CurrentSettings.MusicEnabled = reader.ReadBoolean();
				CurrentSettings.SoundEnabled = reader.ReadBoolean();
				CurrentSettings.ResetCurrentSettings();
			}

			return true;
		}
	}

	public class SpectralSettings
	{
		//Default Settings
		private const float MUSIC_VOLUME_SCALE_DEFAULT = 1;
		private const float SOUND_VOLUME_SCALE_DEFAULT = 1;
		private const bool MUSIC_ENABLED_DEFAULT = true;
		private const bool SOUND_ENABLED_DEFAULT = true;

		//Current Non Saved Settings
		public float ActiveMusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
		public float ActiveSoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
		public bool ActiveMusicEnabled = MUSIC_ENABLED_DEFAULT;
		public bool ActiveSoundEnabled = SOUND_ENABLED_DEFAULT;

		//Current Actual Settings
		public float MusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
		public float SoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
		public bool MusicEnabled = MUSIC_ENABLED_DEFAULT;
		public bool SoundEnabled = SOUND_ENABLED_DEFAULT;

		public SpectralSettings()
		{
			ResetSettings();
		}

		public void ResetSettings()
		{
			ActiveMusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
			ActiveSoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
			ActiveMusicEnabled = MUSIC_ENABLED_DEFAULT;
			ActiveSoundEnabled = SOUND_ENABLED_DEFAULT;
			ApplyCurrent();
		}

		public void ResetCurrentSettings()
		{
			ActiveMusicVolumeScale = MusicVolumeScale;
			ActiveSoundVolumeScale = SoundVolumeScale;
			ActiveMusicEnabled = MusicEnabled;
			ActiveSoundEnabled = SoundEnabled;
		}

		public void ApplyCurrent()
		{
			MusicVolumeScale = ActiveMusicVolumeScale;
			SoundVolumeScale = ActiveSoundVolumeScale;
			MusicEnabled = ActiveMusicEnabled;
			SoundEnabled = ActiveSoundEnabled;
		}
	}
}