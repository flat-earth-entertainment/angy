using System;
using System.Linq;
using System.Reflection;
using Utils;

namespace Abilities
{
    [Serializable]
    public abstract class Ability
    {
        public bool IsFinalized { get; protected set; }
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

        public static Ability RandomAbility()
        {
            var abilityTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(Ability).IsAssignableFrom(t) && t != typeof(Ability));

            return (Ability) Activator.CreateInstance(abilityTypes.RandomElement());
        }

        public void Invoke(PlayerView player)
        {
            WasFired = true;
            InvokeAbility(player);
        }

        public void Wrap()
        {
            if (WasFired && !IsFinalized)
            {
                WrapInternal();
            }
        }

        protected virtual void WrapInternal()
        {
        }

        protected abstract void InvokeAbility(PlayerView player);
    }
}