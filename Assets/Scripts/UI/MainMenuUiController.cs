using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [SerializeField]
        private Button creditsButton;

        [Scene]
        [SerializeField]
        private string creditsScene;

        [SerializeField]
        private Button exitButton;

        private void Awake()
        {
            startButton.onClick.AddListener(delegate { SceneManager.LoadScene(startScene); });

            creditsButton.onClick.AddListener(delegate { SceneManager.LoadScene(creditsScene); });

            exitButton.onClick.AddListener(Application.Quit);
        }
    }
}