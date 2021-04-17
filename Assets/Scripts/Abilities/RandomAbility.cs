using Player;
using UI;
using UnityEngine;

namespace Abilities
{
    public class RandomAbility : Ability
    {
        public static Ability DoAndGetRandomAbilityFor(PlayerView player)
        {
            var newAbility = RandomAbility();
            Object.FindObjectOfType<UiController>().DoSlotMachineFor(player, newAbility);
            return newAbility;
        }

        protected override void InvokeAbility(PlayerView player)
        {
        }
    }
}