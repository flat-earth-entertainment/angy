using System;
using System.Linq;
using System.Reflection;
using Logic;
using Photon.Pun;
using Player;
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

        public static Ability InstanceFromCode(AbilityCode abilityCode)
        {
            return abilityCode switch
            {
                AbilityCode.Expand => new ExpandAbility(),
                AbilityCode.FireDash => new FireDashAbility(),
                AbilityCode.IceBlock => new IceBlockAbility(),
                AbilityCode.NoGravity => new NoGravityAbility(),
                AbilityCode.None => null,
                _ => throw new ArgumentOutOfRangeException(nameof(abilityCode), abilityCode, null)
            };
        }

        public static AbilityCode CodeFromInstance(Ability ability)
        {
            return ability switch
            {
                ExpandAbility _ => AbilityCode.Expand,
                FireDashAbility _ => AbilityCode.FireDash,
                IceBlockAbility _ => AbilityCode.IceBlock,
                NoGravityAbility _ => AbilityCode.NoGravity,
                null => AbilityCode.None,
                _ => throw new ArgumentOutOfRangeException(nameof(ability))
            };
        }

        public static Ability RandomAbility()
        {
            var abilityTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(Ability).IsAssignableFrom(t) && t != typeof(Ability));

            return (Ability) Activator.CreateInstance(abilityTypes.RandomElement());
        }

        public void Invoke(PlayerView player)
        {
            PhotonNetwork.AddCallbackTarget(this);
            WasFired = true;
            InvokeAbility(player);
        }

        public void Wrap()
        {
            if (WasFired && !IsFinalized)
            {
                WrapInternal();
            }

            PhotonNetwork.RemoveCallbackTarget(this);
        }

        ~Ability()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        protected virtual void WrapInternal()
        {
        }

        protected abstract void InvokeAbility(PlayerView player);
    }
}