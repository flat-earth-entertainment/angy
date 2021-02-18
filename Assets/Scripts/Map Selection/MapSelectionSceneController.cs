using Config;
using NaughtyAttributes;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map_Selection
{
    public class MapSelectionSceneController : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string rollDiceScene;

        [SerializeField]
        private GameObject mapPreviewViewPrefab;

        [SerializeField]
        private Transform previewsLayoutParent;

        private void Awake()
        {
            foreach (var mapPreview in GameConfig.Instance.MapPreviews)
            {
                Instantiate(mapPreviewViewPrefab, previewsLayoutParent).GetComponent<MapPreviewView>()
                    .Setup(mapPreview);
            }
        }

        private void OnEnable()
        {
            MapPreviewView.MapPreviewSelected += OnMapPreviewSelected;
        }

        private void OnDisable()
        {
            MapPreviewView.MapPreviewSelected -= OnMapPreviewSelected;
        }

        private void OnMapPreviewSelected(MapPreview mapPreview)
        {
            MapPreviewView.MapPreviewSelected -= OnMapPreviewSelected;
            CurrentGameSession.ChosenMap = mapPreview.Scene;
            SceneChanger.ChangeScene(rollDiceScene);
        }
    }
}