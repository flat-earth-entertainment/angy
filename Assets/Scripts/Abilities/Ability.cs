using System;

namespace Abilities
{
    [Serializable]
    public abstract class Ability
    {
        public abstract void InvokeAbility(PlayerView player);
    }
}