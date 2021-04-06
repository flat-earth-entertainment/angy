using ExitGames.Client.Photon;
using Logic;
using UI;
using UnityEngine;
using Utils;

namespace Network
{
    public class SceneSynchronizer : MonoBehaviour
    {
        [SerializeField]
        private SceneChangeType sceneChangeType;

        private void Awake()
        {
            PhotonEventListener.ListenTo(GameEvent.SceneChange,
                delegate(EventData data) { SceneChanger.ChangeScene((string) data.CustomData, sceneChangeType); });
        }
    }
}