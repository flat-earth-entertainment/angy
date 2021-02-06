using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreenTransitionManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI logoText;

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
            var initialPosition = logoText.rectTransform.position;

            logoText.rectTransform.position += Vector3.up * startFromY;
            DOTween.Sequence().Append(logoText.rectTransform.DOMoveY(initialPosition.y, jumpInTime).SetEase(Ease.OutElastic))
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