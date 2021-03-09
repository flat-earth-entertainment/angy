using System;

namespace Abilities
{
    [Serializable]
    public abstract class Ability
    {
        public bool WasFired { get; private set; }
        public bool Active { get; protected set; }
        public bool Finished { get; protected set; }

        public static Ability Copy(Ability ability)
        {
            if (ability == null)
            {
                return null;
            }

            return (Ability) Activator.CreateInstance(ability.GetType());
        }

        public void Invoke(PlayerView player)
        {
            WasFired = true;
            InvokeAbility(player);
        }

        protected abstract void InvokeAbility(PlayerView player);
    }
}