using System.Linq;
using Player;
using Rewired;

namespace GameSession
{
    public abstract class Player
    {
        public int Id { get; }
        public PlayerView RoundPlayerView { get; set; }
        public int PresetIndex { get; protected set; }

        protected Player(int id)
        {
            Id = id;
        }
    }

    public class LocalPlayer : Player
    {
        public Rewired.Player RewiredPlayer => ReInput.players.GetPlayer(_rewiredPlayerId);

        private readonly int _rewiredPlayerId;

        public LocalPlayer(int playerId, int presetIndex, int rewiredPlayerId) : base(playerId)
        {
            _rewiredPlayerId = rewiredPlayerId;
            PresetIndex = presetIndex;
        }
    }

    public class OnlinePlayer : Player
    {
        public static OnlinePlayer SessionPlayerByActorNumber(int actorNumber)
        {
            return CurrentGameSession.Players.FirstOrDefault(p =>
                p is OnlinePlayer onlinePlayer && onlinePlayer.PhotonPlayer.ActorNumber == actorNumber) as OnlinePlayer;
        }

        public Photon.Realtime.Player PhotonPlayer { get; }

        public OnlinePlayer(int playerId, int presetIndex, Photon.Realtime.Player photonPlayer) : base(playerId)
        {
            PresetIndex = presetIndex;
            PhotonPlayer = photonPlayer;
        }
    }
}