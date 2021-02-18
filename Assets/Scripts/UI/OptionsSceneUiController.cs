using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class OptionsSceneUiController : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string mainMenu;

        private void Awake()
        {
            OptionsController.BackButtonClicked = delegate { SceneManager.LoadScene(mainMenu); };
            OptionsController.Show(false);
        }
    }
}