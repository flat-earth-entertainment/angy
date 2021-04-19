using Config;
using GameSession;
using Network;
using Photon.Pun;
using TMPro;
using UI;
using UnityEngine;

namespace Scenes.Victory
{
    public class VictorySceneController : MonoBehaviour
    {
        private static readonly int IdleBlend = Animator.StringToHash("idleBlend");

        [SerializeField]
        private TextMeshProUGUI winnerScoreLine;

        [SerializeField]
        private TextMeshProUGUI loserScoreLine;

        [SerializeField]
        private SkinnedMeshRenderer winner;

        [SerializeField]
        private Animator winnerAnimator;

        [SerializeField]
        private float winnerIdleBlend;

        [SerializeField]
        private SkinnedMeshRenderer loser;

        [SerializeField]
        private Animator loserAnimator;

        [SerializeField]
        private float loserIdleBlend;

        private void Awake()
        {
            winnerAnimator.SetFloat(IdleBlend, winnerIdleBlend);
            loserAnimator.SetFloat(IdleBlend, loserIdleBlend);

            int sum1;
            var sum0 = sum1 = 0;

            for (var i = 0; i < CurrentGameSession.CollectionScores.Scores.Length; i++)
            {
                var mapScore = CurrentGameSession.CollectionScores.Scores[i];
                if (mapScore.Player1Score != null) sum0 += mapScore.Player1Score.Value;
                if (mapScore.Player2Score != null) sum1 += mapScore.Player2Score.Value;
            }

            int winnerId, loserId;
            if (sum0 > sum1)
            {
                winnerId = 0;
                loserId = 1;
            }
            else
            {
                winnerId = 1;
                loserId = 0;
            }

            winnerScoreLine.text = sum0.ToString();
            loserScoreLine.text = sum1.ToString();

            if (!PhotonNetwork.OfflineMode)
            {
                CurrentPlayer.TryAddPoints(PhotonNetwork.IsMasterClient ? sum0 : sum1);
            }

            var winnerMaterials = winner.materials;
            winnerMaterials[0].SetColor("Color_Primary", GameConfig.Instance.PlayerPresets[winnerId].PlayerColor);
            winner.materials = winnerMaterials;

            var loserMaterials = loser.materials;
            loserMaterials[0].SetColor("Color_Primary", GameConfig.Instance.PlayerPresets[loserId].PlayerColor);
            loser.materials = loserMaterials;
        }

        private void Update()
        {
            if (Input.anyKey)
            {
                SceneChanger.ChangeScene(GameConfig.Instance.Scenes.MainMenuScene);
                CurrentGameSession.ClearSession();
                PhotonNetwork.Disconnect();
                SessionHandler.CancelSession();
            }
        }
    }
}