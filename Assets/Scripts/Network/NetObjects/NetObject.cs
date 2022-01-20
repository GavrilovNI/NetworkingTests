using System;
using UnityEngine;

namespace Network.NetObjects
{
    public abstract class NetObject : MonoBehaviour
    {
        private bool _initialized;

        public int Id { get; private set; }

        public void Initialize(int id)
        {
            if (_initialized)
                throw new Exception("NetObject already initialized.");

            Id = id;
            _initialized = true;
        }
    }
}
