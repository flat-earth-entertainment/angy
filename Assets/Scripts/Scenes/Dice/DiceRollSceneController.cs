using System;
using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ExitGames.Client.Photon;
using GameSession;
using Logic;
using Rewired;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Scenes.Dice
{
    public class DiceRollSceneController : MonoBehaviour
    {
        [SerializeField]
        private Image dieImage;

        [SerializeField]
        private TextMeshProUGUI currentPlayerText;

        [SerializeField]
        private Image player1Image;

        [SerializeField]
        private Image player2Image;

        [SerializeField]
        private Sprite[] diceFaces;

        [SerializeField]
        private float dieFaceChangeInterval = .5f;

        [SerializeField]
        private GameObject player1First;

        private bool _allPlayersRolled;
        private bool _player1Rolled;
        private int _player1Value;
        private bool _shouldChangeFaces;
        private float _timer;
        private Sequence _middleImageFaceChangeSequence;

        private void Awake()
        {
            player1Image.gameObject.SetActive(false);
            player2Image.gameObject.SetActive(false);

            player1First.SetActive(false);

            PhotonEventListener.ListenTo(GameEvent.PlayerOrderSet,
                delegate(EventData data)
                {
                    // CurrentGameSession.
                });

            PhotonEventListener.ListenTo(GameEvent.PlayerSelectedDice, delegate(EventData data)
            {
                
            }, false);
        }

        private void Start()
        {
            _shouldChangeFaces = true;
            currentPlayerText.text = "Player 1";

            _middleImageFaceChangeSequence = FaceChangeSequence(dieImage).Pause();
        }

        // private void

        private Sequence FaceChangeSequence(Image image)
        {
            return DOTween.Sequence()
                .AppendCallback(delegate
                {
                    var dieImageSprites = diceFaces.Where(f => f != image.sprite).ToArray();
                    image.sprite = dieImageSprites[Random.Range(0, dieImageSprites.Length)];
                })
                .AppendInterval(dieFaceChangeInterval)
                .SetUpdate(true)
                .SetLoops(-1);
        }

        private async void Update()
        {
            if (_shouldChangeFaces)
            {
                if (_timer >= dieFaceChangeInterval)
                {
                    var dieImageSprites = diceFaces.Where(f => f != dieImage.sprite).ToArray();
                    dieImage.sprite = dieImageSprites[Random.Range(0, dieImageSprites.Length)];
                    _timer = 0f;
                }

                _timer += Time.deltaTime;
            }


            if (!_allPlayersRolled && (Input.anyKeyDown || ReInput.players.Players.Any(p => p.GetAnyButton())))
            {
                if (!_player1Rolled)
                {
                    _shouldChangeFaces = false;
                    _player1Value = Random.Range(1, 6);
                    var rolledFace = diceFaces[_player1Value];
                    dieImage.sprite = rolledFace;
                    await UniTask.Delay(TimeSpan.FromSeconds(2f));
                    player1Image.sprite = rolledFace;
                    player1Image.gameObject.SetActive(true);
                    _shouldChangeFaces = true;
                    _player1Rolled = true;
                    currentPlayerText.text = "Player 2";
                }
                else
                {
                    _shouldChangeFaces = false;
                    var rolledFace = diceFaces[Random.Range(0, _player1Value)];
                    dieImage.sprite = rolledFace;
                    _allPlayersRolled = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(2f));
                    player2Image.sprite = rolledFace;
                    player2Image.gameObject.SetActive(true);
                    dieImage.gameObject.SetActive(false);
                    player1First.SetActive(true);
                    await UniTask.Delay(TimeSpan.FromSeconds(2f));
                    SceneChanger.ChangeScene(CurrentGameSession.MapCollection.Maps[0]);
                }
            }
        }
    }
}