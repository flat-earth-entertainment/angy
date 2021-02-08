using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreenTransitionManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform logoObject;

        [SerializeField]
        private int startFromY;

        [SerializeField]
        private float jumpInTime;

        [SerializeField]
        private float waitAfterAnimationTime;

        [Scene]
        [SerializeField]
        private string nextScene;

        private void Start()
        {
            var initialPosition = logoObject.position;

            logoObject.position += Vector3.up * startFromY;
            DOTween.Sequence()
                .Append(logoObject.DOMoveY(initialPosition.y, jumpInTime).SetEase(Ease.OutElastic))
                .AppendInterval(waitAfterAnimationTime)
                .AppendCallback(delegate { SceneManager.LoadScene(nextScene); });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Start();
            }
        }
    }
}