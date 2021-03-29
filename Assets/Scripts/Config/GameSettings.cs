using System;
using UnityEngine;

namespace Config
{
    public static class GameSettings
    {
        public enum Settings
        {
            MasterVolume = 0,
            MusicVolume = 1,
            SfxVolume = 2
        }

        public static Action<Settings, float> FloatChanged;

        public static void SetFloat(Settings setting, float value)
        {
            PlayerPrefs.SetFloat(setting.ToString(), value);
            FloatChanged?.Invoke(setting, value);
        }

        public static float GetFloat(Settings setting)
        {
            return PlayerPrefs.GetFloat(setting.ToString(), 1);
        }
    }
}