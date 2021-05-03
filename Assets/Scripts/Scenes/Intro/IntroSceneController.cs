using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UI;
using UnityEngine;
using UnityEngine.Video;

namespace Scenes.Intro
{
    public class IntroSceneController : MonoBehaviour
    {
        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        [Scene]
        private string nextScene;

        [SerializeField]
        private float timeBeforeCanSkip;

        private bool _canSkip;

        private async void Awake()
        {
            videoPlayer.loopPointReached += OnVideoFinished;

            await UniTask.Delay(TimeSpan.FromSeconds(timeBeforeCanSkip), DelayType.Realtime);
            _canSkip = true;
        }

        private void OnVideoFinished(VideoPlayer source)
        {
            SceneChanger.ChangeScene(nextScene);
        }

        private void Update()
        {
            if (_canSkip && Input.anyKeyDown)
            {
                OnVideoFinished(null);
            }
        }
    }
}