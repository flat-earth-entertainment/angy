using System.Linq;
using NaughtyAttributes;
using Rewired;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbySceneController : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string gameScene;

        private void Update()
        {
            if (Input.anyKeyDown || ReInput.players.GetPlayers().Any(p => p.GetAnyButton()))
            {
                SceneChanger.ChangeScene(gameScene);
            }
        }
    }
}