using System;

namespace Player.Input
{
    public interface IPlayerInputs
    {
        public event Action AbilityButtonPressed;
        public event Action MapViewButtonPressed;
    }
}