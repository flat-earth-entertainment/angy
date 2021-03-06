﻿using System;

namespace Player.Input
{
    public interface IPlayerInputs
    {
        public event Action AbilityButtonPressed;
        public event Action MapViewButtonPressed;
        public event Action MenuButtonPressed;
        public event Action FireButtonPressed;
        public event Action<float> HorizontalAxisInput;
        public event Action<float> VerticalAxisInput;
    }
}