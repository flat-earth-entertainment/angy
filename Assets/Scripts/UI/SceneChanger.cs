using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

        public static async void ChangeScene(string sceneName,
            SceneChangeType sceneChangeType = SceneChangeType.Default)
        {
            if (_isCurrentlyChanging)
                return;

            Instance.gameObject.SetActive(true);
            Instance.logo.SetActive(false);
            Instance.cutout.gameObject.SetActive(false);

            var sceneLoad = SceneManager.LoadSceneAsync(sceneName);
            sceneLoad.allowSceneActivation = false;
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
                    Instance.cutout.gameObject.SetActive(true);
                    Instance.cutout.sizeDelta = new Vector2(4000, 4000);
                    await Instance.cutout.DOSizeDelta(Vector2.zero, Instance.transitionTime)
                        .SetEase(Ease.Linear)
                        .SetUpdate(true);
                    break;
            }


            sceneLoad.allowSceneActivation = true;

            await UniTask.NextFrame();

            Destroy(Instance.gameObject);
            _instance = null;
            _isCurrentlyChanging = false;
        }
    }

    public enum SceneChangeType
    {
        Default,
        MapChange
    }
}