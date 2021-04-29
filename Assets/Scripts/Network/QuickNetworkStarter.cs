using System.Collections.Generic;
using System.Linq;
using Config;
using ExitGames.Client.Photon;
using GameSession;
using Logic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;

namespace Network
{
    public class QuickNetworkStarter : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private int mapCollectionId;

        private void InitializeGameSession(EventData eventData)
        {
            var players = new List<GameSession.Player>();

            var dataArray = (object[]) eventData.CustomData;
            CurrentGameSession.MapCollection = GameConfig.Instance.MapCollections[(byte) dataArray[1]];

            for (var i = 0; i < (byte) dataArray[0]; i++)
            {
                var playerId = (byte) dataArray[1 + i * 2];
                var photonActorNumber = (int) dataArray[2 + i * 2];

                if (PhotonNetwork.LocalPlayer.ActorNumber == photonActorNumber)
                {
                    players.Add(new LocalPlayer(playerId, playerId, 0));
                }
                else
                {
                    players.Add(new OnlinePlayer(playerId, playerId,
                        PhotonNetwork.CurrentRoom.GetPlayer(photonActorNumber)));
                }
            }

            CurrentGameSession.Players = players.ToArray();
            CurrentGameSession.SetNextRoundPlayer(players[0]);
            PhotonNetwork.LoadLevel(GameConfig.Instance.MapCollections[mapCollectionId].Maps[0]);
        }

        private void Start()
        {
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = GameConfig.Instance.TimeScale - 0.1f;
            PhotonNetwork.SerializationRate = 20;
            PhotonEventListener.ListenTo(GameEvent.GameSessionPlayersShouldInitialize, InitializeGameSession);
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            PhotonNetwork.JoinOrCreateRoom("quick test", new RoomOptions(), TypedLobby.Default);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                //Add player count (byte)
                var eventData = new List<object>
                {
                    PhotonNetwork.CurrentRoom.PlayerCount
                };

                //Add (byte)game session player id + (int)photon player id
                for (byte i = 0; i < PhotonNetwork.CurrentRoom.Players.Values.Count; i++)
                {
                    eventData.Add(i);
                    eventData.Add(PhotonNetwork.CurrentRoom.Players.Values.ElementAt(i).ActorNumber);
                }

                PhotonShortcuts.ReliableRaiseEventToAll(GameEvent.GameSessionPlayersShouldInitialize, eventData);
            }
        }
    }
}