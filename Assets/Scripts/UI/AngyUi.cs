using System;
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
}