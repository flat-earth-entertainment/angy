using System;
using Abilities;
using Abilities.Config;
using Audio;
using Config;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class AngyUi
    {
        [SerializeField]
        private Slider angySlider;

        [SerializeField]
        private ParticleSystem particle1;

        [SerializeField]
        private ParticleSystem particle2;

        private Tween _sliderSmoothingTween;
        private Sequence _particleSequence;

        private ParticleSystem.MainModule _particle1Main;
        private ParticleSystem.MainModule _particle2Main;

        public void Initialize(float minValue, float maxValue)
        {
            particle1.Stop(true);
            angySlider.minValue = minValue;
            angySlider.maxValue = maxValue;

            _particle1Main = particle1.main;
            _particle2Main = particle2.main;
        }

        public void OnAngyChanged(int newValue)
        {
            if (_sliderSmoothingTween != null)
            {
                _sliderSmoothingTween.Kill();
                _sliderSmoothingTween = null;
            }

            _sliderSmoothingTween = DOTween.To(() => angySlider.value, v => angySlider.value = v, newValue,
                GameConfig.Instance.SliderMoveInterval).SetUpdate(true);


            if (newValue > angySlider.value)
            {
                var particle1MainStartSpeed = _particle1Main.startSpeed;
                particle1MainStartSpeed.constant = Mathf.Abs(particle1MainStartSpeed.constant);
                _particle1Main.startSpeed = particle1MainStartSpeed;

                var particle2MainStartSpeed = _particle2Main.startSpeed;
                particle2MainStartSpeed.constant = Mathf.Abs(particle2MainStartSpeed.constant);
                _particle2Main.startSpeed = particle2MainStartSpeed;
            }

            if (newValue < angySlider.value)
            {
                var particle1MainStartSpeed = _particle1Main.startSpeed;
                particle1MainStartSpeed.constant = -Mathf.Abs(particle1MainStartSpeed.constant);
                _particle1Main.startSpeed = particle1MainStartSpeed;

                var particle2MainStartSpeed = _particle2Main.startSpeed;
                particle2MainStartSpeed.constant = -Mathf.Abs(particle2MainStartSpeed.constant);
                _particle2Main.startSpeed = particle2MainStartSpeed;
            }

            if (_particleSequence != null)
            {
                _particleSequence.Kill();
                _particleSequence = null;
            }

            particle1.Play(true);
            _particle1Main.loop = true;
            _particle2Main.loop = true;
            _particleSequence = DOTween.Sequence()
                .AppendInterval(GameConfig.Instance.SliderMoveInterval)
                .AppendCallback(delegate
                {
                    _particle1Main.loop = false;
                    _particle2Main.loop = false;
                })
                .SetUpdate(true);
        }
    }

    public class UiController : MonoBehaviour
    {
        [SerializeField]
        private GameObject cameraModeWarning;

        [SerializeField]
        private GameObject angyMeter;

        [SerializeField]
        private AngyUi angyUi1;

        [SerializeField]
        private AngyUi angyUi2;

        [SerializeField]
        private PlayersManager playersManager;

        [SerializeField]
        private AbilityUi[] abilityUis;

        private float _angy1Value;
        private float _angy2Value;

        public void WobbleAbilityUi(PlayerView playerView, bool state)
        {
            if (playerView.PlayerId < abilityUis.Length)
            {
                abilityUis[playerView.PlayerId].Wobble = state;
            }
            else
            {
                Debug.LogWarning($"Can't find Ability UI for player with ID {playerView.PlayerId}");
            }
        }

        public void DoSlotMachineFor(PlayerView player, Ability ability)
        {
            abilityUis[player.PlayerId].Visible = true;
            abilityUis[player.PlayerId]
                .DoSlotMachine(3.2f, AbilityConfig.GetConfigSpriteFor(ability), player.PlayerColor);
            AudioManager.PlaySfx(SfxType.RandomActivate);
        }

        private void SetAbilityIconFor(PlayerView player, Sprite icon)
        {
            if (player.PlayerId < abilityUis.Length)
            {
                abilityUis[player.PlayerId].SetAbilityIcon(icon, player.PlayerColor);
            }
            else
            {
                Debug.LogWarning($"Can't find Ability UI for player with ID {player.PlayerId}");
            }
        }

        private void Awake()
        {
            angyUi1.Initialize(GameConfig.Instance.AngyValues.MinAngy, GameConfig.Instance.AngyValues.MaxAngy);
            angyUi2.Initialize(GameConfig.Instance.AngyValues.MinAngy, GameConfig.Instance.AngyValues.MaxAngy);

            playersManager.InitializedAllPlayers += OnPlayersInitialized;
        }

        private void OnEnable()
        {
            PlayerView.NewAbilitySet += OnNewAbilitySet;
        }

        private void OnDisable()
        {
            PlayerView.NewAbilitySet -= OnNewAbilitySet;
        }

        private void OnNewAbilitySet(PlayerView player, Ability ability)
        {
            if (ability == null && player.PreviousAbility != null)
            {
                SetAbilityIconFor(player, AbilityConfig.GetConfigSpriteFor(player.PreviousAbility));
            }
            else
            {
                SetAbilityIconFor(player, AbilityConfig.GetConfigSpriteFor(ability));
            }

            if (player.PlayerState == PlayerState.ActiveInMotion)
            {
                WobbleAbilityUi(player, ability != null);
            }
        }

        private void OnPlayersInitialized(PlayerView[] obj)
        {
            playersManager.InitializedAllPlayers -= OnPlayersInitialized;

            obj[0].AngyChanged += angyUi1.OnAngyChanged;
            obj[1].AngyChanged += angyUi2.OnAngyChanged;
        }

        public void SetCameraModeActive(bool state)
        {
            cameraModeWarning.SetActive(state);
        }

        public void HideAllUi()
        {
            DisableAngyMeter();
            cameraModeWarning.SetActive(false);
            DisableAbilityUi();
        }

        public void EnableAngyMeter()
        {
            angyMeter.SetActive(true);
        }

        public void DisableAngyMeter()
        {
            angyMeter.SetActive(false);
        }


        public void EnableAbilityUi()
        {
            foreach (var abilityUi in abilityUis)
            {
                abilityUi.Visible = true;
            }
        }

        public void DisableAbilityUi()
        {
            foreach (var abilityUi in abilityUis)
            {
                abilityUi.Visible = false;
            }
        }
    }
}