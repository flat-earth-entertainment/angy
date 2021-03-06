using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CreditsUiController : MonoBehaviour
    {
        [SerializeField]
        private Button backButton;

        [Scene]
        [SerializeField]
        private string backScene;

        private void Awake()
        {
            backButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(backScene); });
        }
    }
}