using ExitGames.Client.Photon;
using Logic;
using Photon.Pun;
using Photon.Realtime;

namespace Utils
{
    public static class PhotonShortcuts
    {
        private static readonly RaiseEventOptions DefaultToAll = new RaiseEventOptions {Receivers = ReceiverGroup.All};

        public static bool ReliableRaiseEventToAll(GameEvent gameEvent, object data = null)
        {
            return PhotonNetwork.RaiseEvent((byte) gameEvent, data, DefaultToAll, SendOptions.SendReliable);
        }

        public static bool ReliableRaiseEventToOthers(GameEvent gameEvent, object data = null)
        {
            return PhotonNetwork.RaiseEvent((byte) gameEvent, data, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
    }
}