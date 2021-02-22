using Config;
using NaughtyAttributes;
using UI;
using UnityEngine;

namespace Scenes.Map_Selection
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
            foreach (var mapPreview in GameConfig.Instance.MapCollections)
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

        private void OnMapPreviewSelected(MapCollection mapCollection)
        {
            MapPreviewView.MapPreviewSelected -= OnMapPreviewSelected;
            CurrentGameSession.MapCollection = mapCollection;
            SceneChanger.ChangeScene(rollDiceScene);
        }
    }
}