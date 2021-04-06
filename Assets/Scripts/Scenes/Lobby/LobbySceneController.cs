using System;
using System.Linq;
using ExitGames.Client.Photon;
using Logic;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using Rewired;
using UI;
using UnityEngine;
using Utils;

namespace Scenes.Lobby
{
    public class LobbySceneController : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string gameScene;

        private void Awake()
        {
            PhotonEventListener.ListenTo(GameEvent.PlayerClickedContinueFromLobby,
                delegate { SceneChanger.ChangeScene(gameScene); });
        }

        private void Update()
        {
            if (Input.anyKeyDown || ReInput.players.GetPlayers().Any(p => p.GetAnyButton()))
            {
                PhotonNetwork.RaiseEvent(
                    GameEvent.PlayerClickedContinueFromLobby.ToByte(),
                    null,
                    new RaiseEventOptions {Receivers = ReceiverGroup.All},
                    SendOptions.SendReliable);
            }
        }
    }
}