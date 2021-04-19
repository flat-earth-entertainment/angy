using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class SessionHandler : MonoBehaviour
    {
        public static bool IsSessionInitialized => FindObjectOfType<SessionHandler>();

        public static void Create()
        {
            var sessionHandler = new GameObject().AddComponent<SessionHandler>();
            sessionHandler.gameObject.name = "Session Handler";
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = "Session Handler";
        }

        private void OnApplicationQuit()
        {
            CancelSession();
        }

        public static void CancelSession()
        {
            UnityWebRequest.Get(ServerSettings.LogoutUri).SendWebRequest();
            var sessionHandler = FindObjectOfType<SessionHandler>();
            if (sessionHandler)
                Destroy(sessionHandler.gameObject);
        }
    }
}