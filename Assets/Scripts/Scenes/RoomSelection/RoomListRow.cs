using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.RoomSelection
{
    public class RoomListRow : MonoBehaviour
    {
        public Action Selected;

        [SerializeField]
        private TextMeshProUGUI roomName;

        [SerializeField]
        private TextMeshProUGUI playerCount;

        [SerializeField]
        private TextMeshProUGUI roomState;

        [SerializeField]
        private Button selectButton;

        private void Awake()
        {
            selectButton.onClick.AddListener(delegate { Selected?.Invoke(); });
        }

        public string RoomName
        {
            get => roomName.text;
            set => roomName.text = value;
        }

        public string PlayerCount
        {
            get => playerCount.text;
            set => playerCount.text = value;
        }

        public string RoomState
        {
            get => roomState.text;
            set => roomState.text = value;
        }
    }
}