using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField]
        private GameObject logo;

        [SerializeField]
        private RectTransform cutout;

        [SerializeField]
        private float endScale;

        [SerializeField]
        private float transitionTime;

        public static async void ChangeScene(string sceneName,
            SceneChangeType sceneChangeType = SceneChangeType.Default)
        {
            Instance.gameObject.SetActive(true);
            Instance.logo.SetActive(false);
            Instance.cutout.gameObject.SetActive(false);

            var sceneLoad = SceneManager.LoadSceneAsync(sceneName);
            sceneLoad.allowSceneActivation = false;

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
        }

        public static SceneChanger Instance
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

        private static SceneChanger _instance;
    }

    public enum SceneChangeType
    {
        Default,
        MapChange
    }
}