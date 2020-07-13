using System.IO;
using UnityEngine;

namespace Spectral.Runtime
{
	public static class PersistentDataManager
	{
		private const int EXPECTED_SETTINGS_FILE_LENGTH = 10; //2 Floats, 2 Bools
		private const int EXPECTED_DATA_FILE_LENGTH = 4;      //1 Integer
		private const string SAVE_FOLDER_PATH = "Data";
		private const string DOT = ".";
		private const string FILE_EXTENSION = "data";

		private const string SETTINGS_FILE_NAME = "SpectralSettings";
		private const string DATA_FILE_NAME = "SpectralData";

		public static readonly SpectralSettings CurrentSettings = new SpectralSettings();
		public static readonly SpectralPlayerData CurrentPlayerData = new SpectralPlayerData();

		public static void Initialise()
		{
			if (!ReadSettings())
			{
				SaveOrCreateSettings();
			}

			if (!ReadPlayerData())
			{
				SaveOrCreatePlayerData();
			}
		}

		#region Settings

		public static void SaveOrCreateSettings()
		{
			string folderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH);
			string settingsFilePath = Path.Combine(folderPath, SETTINGS_FILE_NAME + DOT + FILE_EXTENSION);
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

#if SPECTRAL_DEBUG
			Debug.Log($"Saving at: {settingsFilePath}");
#endif
			using (FileStream fileStream = File.Open(settingsFilePath, FileMode.Create))
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
			string settingsFilePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH, SETTINGS_FILE_NAME + DOT + FILE_EXTENSION);
			if (!File.Exists(settingsFilePath))
			{
				return false;
			}

#if SPECTRAL_DEBUG
			Debug.Log($"Reading at: {settingsFilePath}");
#endif
			using (FileStream fileStream = File.Open(settingsFilePath, FileMode.Open))
			{
				if (fileStream.Length != EXPECTED_SETTINGS_FILE_LENGTH)
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

		#endregion

		#region PlayerData

		public static void SaveOrCreatePlayerData()
		{
			string folderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH);
			string dataFilePath = Path.Combine(folderPath, DATA_FILE_NAME + DOT + FILE_EXTENSION);
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

#if SPECTRAL_DEBUG
			Debug.Log($"Saving at: {dataFilePath}");
#endif
			using (FileStream fileStream = File.Open(dataFilePath, FileMode.Create))
			{
				BinaryWriter writer = new BinaryWriter(fileStream);
				writer.Write(CurrentPlayerData.HighestScore);
			}
		}

		private static bool ReadPlayerData()
		{
			string dataFilePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH, DATA_FILE_NAME + DOT + FILE_EXTENSION);
			if (!File.Exists(dataFilePath))
			{
				return false;
			}

#if SPECTRAL_DEBUG
			Debug.Log($"Reading at: {dataFilePath}");
#endif
			using (FileStream fileStream = File.Open(dataFilePath, FileMode.Open))
			{
				if (fileStream.Length != EXPECTED_DATA_FILE_LENGTH)
				{
					fileStream.Close();

					return false;
				}

				BinaryReader reader = new BinaryReader(fileStream);
				CurrentPlayerData.HighestScore = reader.ReadInt32();
			}

			return true;
		}

		#endregion
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

	public class SpectralPlayerData
	{
		public int HighestScore;
	}
}