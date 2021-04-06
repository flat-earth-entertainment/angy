using System.Linq;
using Player;
using Rewired;

namespace GameSession
{
    public abstract class Player
    {
        public int Id { get; }
        public PlayerView RoundPlayerView { get; set; }
        public int PresetIndex { get; set; }

        protected Player(int id)
        {
            Id = id;
        }
    }

    public class LocalPlayer : Player
    {
        public Rewired.Player RewiredPlayer { get; set; }

        public LocalPlayer(int playerId, int presetIndex, int rewiredPlayerId) : base(playerId)
        {
            PresetIndex = presetIndex;
            RewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
        }
    }

    public class OnlinePlayer : Player
    {
        public static OnlinePlayer SessionPlayerByActorNumber(int actorNumber)
        {
            return CurrentGameSession.Players.First(p =>
                p is OnlinePlayer onlinePlayer && onlinePlayer.PhotonPlayer.ActorNumber == actorNumber) as OnlinePlayer;
        }

        public Photon.Realtime.Player PhotonPlayer { get; set; }

        public OnlinePlayer(int playerId, int presetIndex, Photon.Realtime.Player photonPlayer) : base(playerId)
        {
            PresetIndex = presetIndex;
            PhotonPlayer = photonPlayer;
        }
    }
}