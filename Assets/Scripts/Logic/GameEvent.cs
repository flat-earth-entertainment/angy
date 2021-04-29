using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Logic
{
    public enum GameEvent : byte
    {
        PlayerClickedContinueFromLobby,
        GameSessionPlayersShouldInitialize,
        MapCollectionSelected,
        PlayerStartedRollingDice,
        PlayerSelectedDice,
        PlayerOrderSet,
        PlayerStateChanged,
        PlayerStartedPowerMode,
        PlayerCancelledPowerMode,
        PlayerShot,
        PlayerBecameStill,
        PlayerWentOutOfBounds,
        PlayerHitKillTrigger,
        PlayerAngyChanged,
        PlayerAbilitySet,
        PlayerAbilityFired,
        PlayerAbilityButtonPressed,
        PlayerAbilityCancelled,
        SceneChange
    }

    public enum AbilityCode : byte
    {
        None,
        Expand,
        FireDash,
        IceBlock,
        NoGravity,
        Random
    }

    public interface IListener
    {
        public void Invoke();
    }

    public class NetworkListener : IListener
    {
        private readonly byte _eventCode;

        public NetworkListener(byte eventCode)
        {
            _eventCode = eventCode;
        }

        public void Invoke()
        {
            PhotonNetwork.RaiseEvent(_eventCode,
                null,
                new RaiseEventOptions {Receivers = ReceiverGroup.Others},
                SendOptions.SendReliable);
        }
    }

    public interface ISignal
    {
    }

    public class SignalProvider
    {
        private readonly Dictionary<Type, List<IListener>> _subscribers = new Dictionary<Type, List<IListener>>();

        public void RaiseSignal<T>()
        {
            var signalType = typeof(T);
            if (_subscribers.ContainsKey(signalType))
            {
                foreach (var listener in _subscribers[signalType])
                {
                    listener?.Invoke();
                }
            }
        }

        public void Subscribe<T>(IListener listener) where T : ISignal
        {
            var signalType = typeof(T);
            if (_subscribers.ContainsKey(signalType))
            {
                _subscribers[signalType].Add(listener);
            }
            else
            {
                _subscribers.Add(signalType, new List<IListener>());
                _subscribers[signalType].Add(listener);
            }
        }

        public void Unsubscribe<T>(IListener listener)
        {
            var signalType = typeof(T);
            if (_subscribers.ContainsKey(signalType))
            {
                _subscribers[signalType].Remove(listener);
            }
        }
    }
}