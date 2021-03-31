using Ball.Utils;
using Cysharp.Threading.Tasks;
using MeshPhysics;

namespace Ball
{
    public class LevelManager : Singleton<LevelManager>
    {
        private bool _lateStart;

        private async void Update()
        {
            if (!_lateStart)
            {
                // Activates only once, frame after start
                _lateStart = true;
                // This one needs to be done after start since the thing its getting is created in start but can't be moved to awake
                await UniTask.WaitUntil(() => FindObjectOfType<MeshCombiner>().Baked);
                PredictionManager.Instance.obstacles = FindObjectOfType<MeshCombiner>().transform.parent.gameObject;
                PredictionManager.Instance.CopyAllObstacles();
            }
        }
    }
}