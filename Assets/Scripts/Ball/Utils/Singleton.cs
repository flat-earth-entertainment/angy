using UnityEngine;

namespace Ball.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static bool Verbose = false;

        private static T _instance;
        public bool keepAlive;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var singletonObj = new GameObject();
                        singletonObj.name = typeof(T).ToString();
                        _instance = singletonObj.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        public static bool IsInstanceAlive => _instance != null;

        public virtual void Awake()
        {
            if (_instance != null)
            {
                if (Verbose)
                    Debug.Log("SingleAccessPoint, Destroy duplicate instance " + name + " of " + Instance.name);
                Destroy(gameObject);
                return;
            }

            _instance = GetComponent<T>();

            if (keepAlive)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (_instance == null)
            {
                if (Verbose)
                    Debug.LogError("SingleAccessPoint<" + typeof(T).Name + "> Instance null in Awake");
                return;
            }

            if (Verbose)
                Debug.Log("SingleAccessPoint instance found " + Instance.GetType().Name);
        }
    }
}