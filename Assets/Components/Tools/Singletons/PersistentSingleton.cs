using UnityEngine;

namespace VComponent.Tools.Singletons
{
    /// <summary>
    /// A singleton that will be persistent across scene by being a DDOL.
    /// </summary>
    [DefaultExecutionOrder(-95)]
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private readonly bool _autoUnParentOnAwake = true;

        private static T _instance;

        public static bool HasInstance => _instance != null;
        public static T TryGetInstance() => HasInstance ? _instance : null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError($"There was no instance of of {typeof(T).Name} and yet someone try to call it. An new instance has been set but singletons should always be present.");
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            if (_autoUnParentOnAwake)
            {
                transform.SetParent(null);
            }

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (_instance != this)
                {
                    //Debug.LogWarning($"There was already an instance of {typeof(T).Name} new initialization abort and doublon destroyed.");
                    Destroy(gameObject);
                }
            }
        }
    }
}