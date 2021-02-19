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
        private float endScale;

        [SerializeField]
        private float transitionTime;

        public static async void ChangeScene(string sceneName)
        {
            Instance.gameObject.SetActive(true);

            var sceneLoad = SceneManager.LoadSceneAsync(sceneName);
            sceneLoad.allowSceneActivation = false;

            Instance.logo.transform.localScale = Vector3.zero;
            await Instance.logo.transform.DOScale(Instance.endScale, Instance.transitionTime)
                .SetEase(Ease.InCubic)
                .SetUpdate(true);
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
}