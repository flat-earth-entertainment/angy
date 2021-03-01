using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUiController : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonListParent;

        [SerializeField]
        private Button startButton;

        [Scene]
        [SerializeField]
        private string startScene;

        [SerializeField]
        private Button helpButton;

        [SerializeField]
        private GameObject helpUiPrefab;

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


        private GameObject _helpUi;

        private void Awake()
        {
            startButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(startScene); });

            helpButton.onClick.AddListener(delegate
            {
                HelpUi.OnClose = HideHelp;
                _helpUi = Instantiate(helpUiPrefab);
                buttonListParent.SetActive(false);
            });

            creditsButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(creditsScene); });

            optionsButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(optionsScene); });

            exitButton.onClick.AddListener(Application.Quit);
        }

        private void HideHelp()
        {
            Destroy(_helpUi);
            _helpUi = null;
            buttonListParent.SetActive(true);
            HelpUi.OnClose = null;
        }
    }
}