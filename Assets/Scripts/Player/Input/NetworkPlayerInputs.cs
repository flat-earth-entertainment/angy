using System;

namespace Player.Input
{
    public class NetworkPlayerInputs : IPlayerInputs
    {
#pragma warning disable 0067
        public event Action AbilityButtonPressed;
        public event Action MapViewButtonPressed;
        public event Action MenuButtonPressed;
        public event Action FireButtonPressed;
        public event Action<float> HorizontalAxisInput;
        public event Action<float> VerticalAxisInput;
#pragma warning restore 0067
    }
}