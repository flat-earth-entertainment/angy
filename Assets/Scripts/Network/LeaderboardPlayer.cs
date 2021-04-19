using UnityEngine;

namespace Network
{
    public class LeaderboardPlayer
    {
        public string Nickname { get; }
        public PlayerAvatar Avatar { get; }
        public int Highscore { get; }

        public LeaderboardPlayer(string nickname, PlayerAvatar avatar, int highscore)
        {
            Nickname = nickname;
            Avatar = avatar;
            Highscore = highscore;
        }

        public static bool TryParseFromServerResponse(string serverResponse, out LeaderboardPlayer leaderboardPlayer)
        {
            var playerData = serverResponse.Split(':');
            if (playerData.Length != 4)
            {
                leaderboardPlayer = default;
                return false;
            }

            var name = playerData[0];
            var avatar = new PlayerAvatar(float.Parse(playerData[1].Replace('.', ',')),
                float.Parse(playerData[2].Replace('.', ',')));
            var score = int.Parse(playerData[3]);

            leaderboardPlayer = new LeaderboardPlayer(name, avatar, score);
            return true;
        }
    }

    public readonly struct PlayerAvatar
    {
        public readonly float Hue;
        public readonly float Saturation;

        public PlayerAvatar(float hue, float saturation)
        {
            Hue = hue;
            Saturation = saturation;
        }

        public Color AsColor() => Color.HSVToRGB(Hue, Saturation, 1);
    }
}