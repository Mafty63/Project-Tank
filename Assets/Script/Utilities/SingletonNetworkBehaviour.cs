using UnityEngine;
using Unity.Netcode;

namespace ProjectTank.Utilities
{
    public class SingletonNetworkBehaviour<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }
    }
}
