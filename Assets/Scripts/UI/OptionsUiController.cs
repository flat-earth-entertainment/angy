using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class OptionsUiController : MonoBehaviour
    {
        [SerializeField]
        private Button backButton;

        [Scene]
        [SerializeField]
        private string backScene;

        private void Awake()
        {
            backButton.onClick.AddListener(delegate { SceneManager.LoadScene(backScene); });
        }
    }
}