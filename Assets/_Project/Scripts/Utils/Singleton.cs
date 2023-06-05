using UnityEngine;

namespace BobIO
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            CreateSingletone();
            DontDestroyOnLoad(gameObject);
        }

        private void CreateSingletone()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }
        }
    }
}