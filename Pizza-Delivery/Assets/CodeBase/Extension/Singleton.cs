using UnityEngine;

namespace CodeBase.Extension
{
    [RequireComponent(typeof(Initialization))]
    public abstract class Singleton<T> : MonoBehaviour
    {
        //===================================================================================

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("Singleton not registered! Make sure the GameObject running your singleton is active in your scene and has an Initialization component attached.");
                    return default(T);
                }
                return _instance;
            }
        }

        [SerializeField] bool _dontDestroyOnLoad = false;
        static T _instance;

        //===================================================================================

        protected virtual void OnRegistration()
        {
        }

        //===================================================================================

        public void RegisterSingleton(T instance)
        {
            _instance = instance;
        }

        //===================================================================================

        protected void Initialize(T instance)
        {
            if (_dontDestroyOnLoad && _instance != null)
            {
                Destroy(gameObject);
                return;
            }

            if (_dontDestroyOnLoad)
            {
                //don't destroy on load only works on root objects:
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }

            _instance = instance;
            OnRegistration();
        }

        //===================================================================================
    }
}