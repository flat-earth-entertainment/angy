using System;
using UnityEngine;

namespace Audio
{
    public class AudioSystemSignalSender : MonoBehaviour
    {
        // Used from inspector
        // ReSharper disable once UnusedMember.Global
        public void PlaySfx(string sfxTypeString)
        {
            if (Enum.TryParse<SfxType>(sfxTypeString, true, out var sfxType))
            {
                AudioManager.PlaySfx(sfxType);
            }
            else
            {
                Debug.LogError("Can't find sfx type " + sfxTypeString + "!");
            }
        }
    }
}