using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Audio
{
#if UNITY_EDITOR
    public static class SelfLoader
    {
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void SelfInstantiate()
        {
            UnityEditor.SceneManagement.EditorSceneManager.sceneLoaded += delegate
            {
                if (AudioManager.Instance)
                {
                }
            };
        }
    }
#endif

    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private SfxPreset[] sfxPresets;

        [SerializeField]
        private AudioClip[] music;

        [SerializeField]
        private AudioMixerSnapshot defaultState;

        [SerializeField]
        private AudioMixerSnapshot lowPassState;

        [SerializeField]
        private AudioSource musicSource;

        [SerializeField]
        private AudioSource sfxSource;

        [SerializeField]
        private AudioSource tapSource;

        [SerializeField]
        private AudioMixer masterMixer;

        private readonly Dictionary<GameSettings.Settings, MixerBand> MixerBands =
            new Dictionary<GameSettings.Settings, MixerBand>();

        private class MixerBand
        {
            private const float MinVolumeValueZeroOne = 0.0001f;
            private readonly float _maxVolumeValueZeroOne;

            private string BandName { get; }

            private AudioMixer Mixer { get; }

            public MixerBand(string bandName, AudioMixer mixer)
            {
                BandName = bandName;
                Mixer = mixer;
                mixer.GetFloat(bandName, out _maxVolumeValueZeroOne);
                _maxVolumeValueZeroOne = DbToZeroOne(_maxVolumeValueZeroOne);
            }

            public void SetVolume(float volume)
            {
                volume = ZeroOneToDb(LinearRemap(volume, 0, 1,
                    MinVolumeValueZeroOne, _maxVolumeValueZeroOne));

                Mixer?.SetFloat(BandName, volume);
            }

            private static float DbToZeroOne(float db)
            {
                return Mathf.Pow(10, db / 20);
            }

            private static float ZeroOneToDb(float value)
            {
                return Mathf.Log10(value) * 20;
            }

            private static float LinearRemap(float value,
                float valueRangeMin, float valueRangeMax,
                float newRangeMin, float newRangeMax)
            {
                return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) +
                       newRangeMin;
            }
        }

        public static void PlaySfx(SfxType sfxType)
        {
            var sfxPreset = Instance.sfxPresets.FirstOrDefault(p => p.SfxType == sfxType);

            if (sfxPreset != null)
            {
                var clip = sfxPreset.RandomClip;

                if (clip != null && !clip.Equals(null))
                {
                    switch (sfxType)
                    {
                        case SfxType.LemmingRotate:
                            _instance.tapSource.PlayOneShot(clip);
                            return;
                    }

                    Instance.sfxSource.PlayOneShot(clip);
                }
            }
        }

        public void DoLowPass(float fadeInDuration = 2f)
        {
            lowPassState.TransitionTo(fadeInDuration);
        }

        public void UndoLowPass(float fadeInDuration = 2f)
        {
            defaultState.TransitionTo(fadeInDuration);
        }

        private async void PlayNextMusic()
        {
            var chosenClip = music.Where(c => c != musicSource.clip).RandomElement();

            musicSource.clip = chosenClip;
            musicSource.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(chosenClip.length), DelayType.Realtime);

            PlayNextMusic();
        }

        private static void OnVolumeChanged(GameSettings.Settings setting, float newValue)
        {
            if (Instance.MixerBands.ContainsKey(setting))
            {
                Instance.MixerBands[setting].SetVolume(newValue);
            }
        }

        private void OnEnable()
        {
            GameSettings.FloatChanged += OnVolumeChanged;
        }

        private void OnDisable()
        {
            GameSettings.FloatChanged -= OnVolumeChanged;
        }

        private void Start()
        {
            PlayNextMusic();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            MixerBands.Add(GameSettings.Settings.MasterVolume, new MixerBand("MasterVolume", masterMixer));
            MixerBands.Add(GameSettings.Settings.MusicVolume, new MixerBand("MusicVolume", masterMixer));
            MixerBands.Add(GameSettings.Settings.SfxVolume, new MixerBand("SfxVolume", masterMixer));

            foreach (var mixerBand in MixerBands)
            {
                mixerBand.Value.SetVolume(GameSettings.GetFloat(mixerBand.Key));
            }
        }

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null || _instance.Equals(null))
                {
                    _instance = FindObjectOfType<AudioManager>();

                    if (_instance == null || _instance.Equals(null))
                    {
                        _instance = Instantiate(GameConfig.Instance.AudioManager).GetComponent<AudioManager>();

                        if (_instance == null || _instance.Equals(null))
                        {
                            Debug.LogError("Cannot instantiate Audio Manager! Check the Game Config or call Gabe!");
                        }
                    }
                }

                return _instance;
            }
        }

        private static AudioManager _instance;
    }
}