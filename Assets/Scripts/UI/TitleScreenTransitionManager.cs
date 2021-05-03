using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreenTransitionManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform logoObject;

        [SerializeField]
        private int startFromY;

        [SerializeField]
        private float jumpInTime;

        [Scene]
        [SerializeField]
        private string nextScene;

        private bool _canGoToNextScene;
        private AsyncOperation _mainMenuLoad;

        private async void Start()
        {
            var initialPosition = logoObject.position;

            _mainMenuLoad = SceneManager.LoadSceneAsync(nextScene);
            _mainMenuLoad.allowSceneActivation = false;

            logoObject.position += Vector3.up * startFromY;
            await logoObject.DOMoveY(initialPosition.y, jumpInTime).SetEase(Ease.OutElastic);
            _canGoToNextScene = true;
        }

        private void Update()
        {
            if (_canGoToNextScene && Input.anyKeyDown)
            {
                SceneManager.LoadScene(nextScene);
                _mainMenuLoad.allowSceneActivation = true;
            }
        }
    }
}