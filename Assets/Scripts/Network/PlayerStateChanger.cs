using System.Linq;
using ExitGames.Client.Photon;
using GameSession;
using Logic;
using Player;
using UnityEngine;
using Utils;

namespace Network
{
    public class PlayerStateChanger : MonoBehaviour
    {
        private void Awake()
        {
            PhotonEventListener.ListenTo(GameEvent.PlayerStateChanged, delegate(EventData data)
                {
                    var dataArray = (object[]) data.CustomData;
                    var playerId = (int) dataArray[0];
                    var newState = (PlayerState) (byte) dataArray[1];
                    CurrentGameSession.Players.First(p => p.Id == playerId).RoundPlayerView.PlayerState = newState;
                },
                false);
        }
    }
}