using Config;
using NaughtyAttributes;
using Rewired.Integration.UnityUI;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
                var newMapPreview = Instantiate(mapPreviewViewPrefab, previewsLayoutParent);
                newMapPreview.GetComponent<MapPreviewView>().Setup(mapPreview);

                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(newMapPreview.GetComponentInChildren<Button>()
                        .gameObject);
                }
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