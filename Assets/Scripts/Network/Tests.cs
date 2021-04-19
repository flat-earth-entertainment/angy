using UnityEngine;

namespace Network
{
    public class Tests : MonoBehaviour
    {
        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log(await CurrentPlayer.TryLogin("Gabe", "password"));
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SessionHandler.CancelSession();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log(CurrentPlayer.LeaderboardPlayer.Nickname + " " + CurrentPlayer.LeaderboardPlayer.Highscore);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                CurrentPlayer.TryAddPoints(5);
            }
        }
    }
}