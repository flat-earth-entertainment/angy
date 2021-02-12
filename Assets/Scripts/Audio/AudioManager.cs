using System;
using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
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
        private const string LowPassParameter = "LowPassFrequency";

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
        private AudioMixerGroup audioMixer;

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

        private void Start()
        {
            PlayNextMusic();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
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