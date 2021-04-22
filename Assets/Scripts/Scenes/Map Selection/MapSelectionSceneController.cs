using System.Linq;
using Config;
using ExitGames.Client.Photon;
using GameSession;
using Logic;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

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
            PhotonEventListener.ListenTo(GameEvent.MapCollectionSelected, delegate(EventData data)
            {
                var selectedCollectionName = data.CustomData as string;

                CurrentGameSession.MapCollection =
                    GameConfig.Instance.MapCollections.First(c => c.Name == selectedCollectionName);

                SceneChanger.BroadcastChangeSceneToSceneSync(CurrentGameSession.MapCollection.Maps[0]);
            });

            foreach (var mapPreview in GameConfig.Instance.MapCollections)
            {
                var newMapPreview = Instantiate(mapPreviewViewPrefab, previewsLayoutParent);
                newMapPreview.GetComponent<MapPreviewView>().Setup(mapPreview);

                var button = newMapPreview.GetComponentInChildren<Button>();

                button.interactable = PhotonNetwork.IsMasterClient;

                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(button
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
            PhotonNetwork.RaiseEvent(GameEvent.MapCollectionSelected.ToByte(), mapCollection.Name,
                new RaiseEventOptions {Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
            MapPreviewView.MapPreviewSelected -= OnMapPreviewSelected;
        }
    }
}