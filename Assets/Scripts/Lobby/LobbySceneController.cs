using System.Linq;
using NaughtyAttributes;
using Rewired;
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
                SceneManager.LoadScene(gameScene);
            }
        }
    }
}