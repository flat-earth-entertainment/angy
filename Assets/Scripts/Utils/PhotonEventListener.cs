using System;
using ExitGames.Client.Photon;
using Logic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Utils
{
    public class PhotonEventListener : MonoBehaviour, IOnEventCallback
    {
        private bool _listenOnce;
        private bool _listenedOnce;
        private bool _forceStop;
        private Action<EventData> _onEventAction;
        private byte _eventCode;

        public static PhotonEventListener ListenTo(GameEvent gameEvent, Action<EventData> action,
            bool listenOnce = true)
        {
            var thisObject = new GameObject().AddComponent<PhotonEventListener>();

            thisObject.gameObject.name = gameEvent + " Listener";
            if (listenOnce)
            {
                thisObject.gameObject.name += " One-shot";
            }

            thisObject._onEventAction = action;
            thisObject._eventCode = (byte) gameEvent;
            thisObject._listenOnce = listenOnce;
            return thisObject;
        }

        public void StopListening()
        {
            _forceStop = true;
            Destroy(gameObject);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (_forceStop || photonEvent.Code != _eventCode || _listenOnce && _listenedOnce)
                return;

            _onEventAction?.Invoke(photonEvent);
            _listenedOnce = true;

            if (_listenOnce)
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}