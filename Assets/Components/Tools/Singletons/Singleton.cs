using UnityEngine;

namespace VComponent.Tools.Singletons
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        [Header("Singleton Parameters")]
        [SerializeField, Tooltip("If true will mark the object has DDOL on initialisation.")] 
        private bool _isPersistent;
        
        [SerializeField, Tooltip("If true will parent to the root of the scene. Preferred for persistent singletons.")] 
        private bool _autoUnParentOnAwake = true;
        
        private static T _instance;

        public static bool HasInstance => _instance != null;
        public static T TryGetInstance() => HasInstance ? _instance : null;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                
                _instance = FindAnyObjectByType<T>();
                    
                if (_instance == null)
                {
                    Debug.LogError($"[SINGLETON] There was no instance of {typeof(T).Name} and yet someone try to call it. Ensure that the singleton is always present when needed.");
                }
                else
                {
                    Debug.LogWarning($"[SINGLETON] There was no instance of {typeof(T).Name} referenced, one was found but this behavior is not standard.");
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
            if (!Application.isPlaying) 
                return;

            if (_autoUnParentOnAwake)
            {
                transform.SetParent(null);
            }

            if (_instance == null)
            {
                _instance = this as T;
                transform.name = $"[SINGLETON]_{name}";
                
                if (_isPersistent)
                {
                    DontDestroyOnLoad(gameObject);
                    transform.name = $"[PERSISTENT_SINGLETON]_{name}";
                }
            }
            else
            {
                if (_instance != this)
                {
                    Debug.LogError($"[SINGLETON] There was already an instance of {typeof(T).Name} new initialization abort and doublon destroyed.");
                    Destroy(gameObject);
                }
            }
        }
    }
}