using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class NetworkLeaderboardUiController : MonoBehaviour
    {
        [SerializeField]
        private GameObject linePrefab;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        [Scene]
        private string backScene;

        private async void Awake()
        {
            backButton.onClick.AddListener(delegate { SceneManager.LoadScene(backScene); });

            var getHighscoresListRequest = UnityWebRequest.Post(ServerSettings.ActionUri,
                new List<IMultipartFormSection> {new MultipartFormDataSection("action", "getScores")});

            await getHighscoresListRequest.SendWebRequest();

            if (getHighscoresListRequest.result == UnityWebRequest.Result.ConnectionError ||
                getHighscoresListRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(getHighscoresListRequest.error);
            }
            else
            {
                //Getting the raw rows by splitting the server's response
                var rows = getHighscoresListRequest.downloadHandler
                    .text.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

                //List of scores to sort
                var scores = new List<LeaderboardPlayer>();

                //Convert raw rows to name and score and populate a list with them
                foreach (var row in rows)
                {
                    var tokens = row.Split(':');

                    var newScore = new LeaderboardPlayer(
                        tokens[0],
                        new PlayerAvatar(float.Parse(tokens[2].Replace('.', ',')),
                            float.Parse(tokens[3].Replace('.', ','))),
                        int.Parse(tokens[1]));

                    scores.Add(newScore);
                }

                //Spawn rows to scene
                foreach (var leaderboardPlayer in scores)
                {
                    var newRow = Instantiate(linePrefab, scrollRect.content).GetComponent<NetworkLeaderboardRow>();
                    newRow.Setup(leaderboardPlayer);
                }
            }
        }
    }
}