using Config;
using UI;
using UnityEngine;

namespace Scenes.Victory
{
    public class VictorySceneController : MonoBehaviour
    {
        private static readonly int IdleBlend = Animator.StringToHash("idleBlend");

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
            var winnerMaterials = winner.materials;
            winnerMaterials[0] = CurrentGameSession.WinnerMaterial;
            winner.materials = winnerMaterials;

            var loserMaterials = loser.materials;
            loserMaterials[0] = CurrentGameSession.LoserMaterial;
            loser.materials = loserMaterials;

            winnerAnimator.SetFloat(IdleBlend, winnerIdleBlend);
            loserAnimator.SetFloat(IdleBlend, loserIdleBlend);
        }

        private void Update()
        {
            if (Input.anyKey)
            {
                SceneChanger.ChangeScene(GameConfig.Instance.Scenes.MainMenuScene);
            }
        }
    }
}