using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUiController : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

        [Scene]
        [SerializeField]
        private string startScene;

        [SerializeField]
        private Button optionsButton;

        [Scene]
        [SerializeField]
        private string optionsScene;

        [SerializeField]
        private Button creditsButton;

        [Scene]
        [SerializeField]
        private string creditsScene;

        [SerializeField]
        private Button exitButton;

        private void Awake()
        {
            startButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(startScene); });

            creditsButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(creditsScene); });

            optionsButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(optionsScene); });

            exitButton.onClick.AddListener(Application.Quit);
        }
    }
}