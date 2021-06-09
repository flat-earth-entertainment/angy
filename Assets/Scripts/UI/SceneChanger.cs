using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Logic;
using Photon.Pun;
using Rewired.Integration.UnityUI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI
{
    public class SceneChanger : MonoBehaviour
    {
        private static bool _isCurrentlyChanging;

        private static SceneChanger _instance;

        [SerializeField]
        private GameObject logo;

        [SerializeField]
        private RectTransform cutout;

        [SerializeField]
        private float endScale;

        [SerializeField]
        private float transitionTime;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Transform levelCompleteLogo;

        [SerializeField]
        private TextMeshProUGUI levelCompleteText;

        [SerializeField]
        private float levelCompleteTime;

        [SerializeField]
        private Ease easeType;

        private static SceneChanger Instance
        {
            get
            {
                if (_instance == null || _instance.Equals(null))
                {
                    _instance = FindObjectOfType<SceneChanger>();

                    if (_instance == null || _instance.Equals(null))
                    {
                        _instance = Instantiate(GameConfig.Instance.SceneChanger).GetComponent<SceneChanger>();

                        if (_instance == null || _instance.Equals(null))
                        {
                            Debug.LogError("Cannot instantiate Scene Manager! Check the Game Config or call Gabe!");
                        }
                    }
                }

                return _instance;
            }
        }

        public static void BroadcastChangeSceneToSceneSync(string sceneName,
            SceneChangeType sceneChangeType = SceneChangeType.Default)
        {
            // PhotonEventListener.ListenTo(GameEvent.SceneChange,
            //     delegate(EventData data) { ChangeScene((string) data.CustomData); });
            //
            PhotonShortcuts.ReliableRaiseEventToAll(GameEvent.SceneChange, sceneName);
            // PhotonNetwork.LoadLevel(sceneName);
        }

        public static async void ChangeScene(string sceneName,
            SceneChangeType sceneChangeType = SceneChangeType.Default)
        {
            if (_isCurrentlyChanging)
                return;

            Instance.gameObject.SetActive(true);
            Instance.logo.SetActive(false);
            Instance.cutout.gameObject.SetActive(false);
            Instance.levelCompleteLogo.parent.gameObject.SetActive(false);

            _isCurrentlyChanging = true;

            foreach (var eventSystem in FindObjectsOfType<EventSystem>())
            {
                eventSystem.enabled = false;
            }

            foreach (var eventSystem in FindObjectsOfType<RewiredEventSystem>())
            {
                eventSystem.enabled = false;
            }

            switch (sceneChangeType)
            {
                case SceneChangeType.Default:
                    Instance.logo.SetActive(true);
                    Instance.logo.transform.localScale = Vector3.zero;
                    await Instance.logo.transform.DOScale(Instance.endScale, Instance.transitionTime)
                        .SetEase(Ease.InCubic)
                        .SetUpdate(true);
                    break;

                case SceneChangeType.MapChange:
                    Instance.levelCompleteLogo.parent.gameObject.SetActive(true);

                    Instance.levelCompleteText.text = "Level " + (LevelIndicator.CurrentLevelNumber + 1).ToString();

                    Instance.canvasGroup.alpha = 0f;
                    DOTween.To(() => Instance.canvasGroup.alpha, a => Instance.canvasGroup.alpha = a, 1f, 1f)
                        .SetUpdate(true);

                    var initialScale = Instance.levelCompleteLogo.localScale;
                    var initialRotation = Instance.levelCompleteLogo.rotation;
                    Instance.levelCompleteLogo.localScale = Vector3.zero;

                    Instance.levelCompleteLogo.DOScale(initialScale, Instance.levelCompleteTime / 2);
                    Instance.levelCompleteLogo.DORotate(new Vector3(0, 0, 720 + initialRotation.eulerAngles.z),
                            Instance.levelCompleteTime / 2,
                            RotateMode.FastBeyond360)
                        .SetEase(Instance.easeType);


                    await UniTask.Delay(TimeSpan.FromSeconds(Instance.levelCompleteTime));
                    break;

                case SceneChangeType.Cutout:
                    Instance.cutout.gameObject.SetActive(true);
                    Instance.cutout.sizeDelta = new Vector2(4000, 4000);
                    await Instance.cutout.DOSizeDelta(Vector2.zero, Instance.transitionTime)
                        .SetEase(Ease.Linear)
                        .SetUpdate(true);

                    break;
            }

            PhotonNetwork.LoadLevel(sceneName);
            await UniTask.NextFrame();

            Destroy(Instance.gameObject);
            _instance = null;
            _isCurrentlyChanging = false;
        }
    }

    public enum SceneChangeType
    {
        Default,
        MapChange,
        Cutout
    }
}