using TMPro;
using UnityEngine;

namespace Scenes.RoomLobby
{
    public class PlayerListRow : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI playerName;

        [SerializeField]
        private TextMeshProUGUI playerReadyStatus;

        public string PlayerName
        {
            get => playerName.text;
            set => playerName.text = value;
        }

        public string PlayerReadyStatus
        {
            get => playerReadyStatus.text;
            set => playerReadyStatus.text = value;
        }
    }
}