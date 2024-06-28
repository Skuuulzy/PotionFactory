using UnityEngine;

namespace VComponent.Tools.Singletons
{
    /// <summary>
    /// Persistent Regulator singleton, will destroy any other older components of the same type it finds on awake.
    /// </summary>
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static bool HasInstance => _instance != null;

        public float InitializationTime { get; private set; }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        go.hideFlags = HideFlags.HideAndDontSave;
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
            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);

            T[] oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (T old in oldInstances)
            {
                if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime)
                {
                    Destroy(old.gameObject);
                }
            }

            if (_instance == null)
            {
                _instance = this as T;
            }
        }
    }
}