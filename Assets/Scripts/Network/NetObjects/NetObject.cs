using System;
using UnityEngine;

namespace Network.NetObjects
{
    public abstract class NetObject : MonoBehaviour
    {
        public event Action<NetObject> BeforeDestroyed;

        private bool _initialized;

        public NetworkManager NetworkManager { get; private set; }
        public int Id { get; private set; }

        public void Initialize(int id, NetworkManager networkManager)
        {
            if (_initialized)
                throw new Exception("NetObject already initialized.");

            Id = id;
            NetworkManager = networkManager;
            _initialized = true;
        }

        private void OnDestroy()
        {
            BeforeDestroyed?.Invoke(this);
        }
    }
}
