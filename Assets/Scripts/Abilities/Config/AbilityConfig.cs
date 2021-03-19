using System;
using Config;
using UnityEngine;

namespace Abilities.Config
{
    [Serializable]
    public abstract class AbilityConfig
    {
        [field: SerializeField]
        public Sprite AbilityIcon { get; private set; }

        public static Sprite GetConfigSpriteFor(Ability ability)
        {
            if (ability == null)
            {
                return null;
            }

            return ability switch
            {
                ExpandAbility _ => GameConfig.Instance.AbilityValues.ExpandAbility.AbilityIcon,
                IceBlockAbility _ => GameConfig.Instance.AbilityValues.IceBlockAbility.AbilityIcon,
                NoGravityAbility _ => GameConfig.Instance.AbilityValues.NoGravityAbility.AbilityIcon,
                FireDashAbility _ => GameConfig.Instance.AbilityValues.FireDashAbilityConfig.AbilityIcon,
                _ => throw new ArgumentOutOfRangeException(nameof(ability))
            };
        }
    }
}