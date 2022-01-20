using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Network.NetObjects
{
    public class NetObjectsContainer : MonoBehaviour
    {
        private int _lastId = 0;
        private Dictionary<int, NetObject> _netObjects = new Dictionary<int, NetObject>();

        private void InitializeNetObject(NetObject netObject)
        {
            InitializeNetObject(netObject, _lastId++);
        }
        private void InitializeNetObject(NetObject netObject, int id)
        {
            netObject.Initialize(id);
            _netObjects.Add(netObject.Id, netObject);
            netObject.name = "NetObject(" + netObject.Id + ") [" + netObject.GetType().Name + "]";
        }

        public T CreateNetObject<T>() where T : NetObject
        {
            T netObject = new GameObject().AddComponent<T>();
            InitializeNetObject(netObject);
            return netObject;
        }

        public NetObject CreateNetObject(Type type)
        {
            NetObject netObject = new GameObject().AddComponent(type) as NetObject;
            InitializeNetObject(netObject);
            return netObject;
        }

        public T CreateNetObject<T>(int id) where T : NetObject
        {
            T netObject = new GameObject().AddComponent<T>();
            InitializeNetObject(netObject, id);
            return netObject;
        }

        public NetObject CreateNetObject(Type type, int id)
        {
            NetObject netObject = new GameObject().AddComponent(type) as NetObject;
            InitializeNetObject(netObject, id);
            return netObject;
        }

        public bool HasNetObject(int id)
        {
            return _netObjects.ContainsKey(id);
        }

        public NetObject GetNetObject(int id)
        {
            return _netObjects[id];
        }

        public void DestroyNetObject(int id)
        {
            GameObject.Destroy(_netObjects[id].gameObject);
            _netObjects.Remove(id);
        }

        public void Foreach(Action<NetObject> action)
        {
            foreach(var netObject in _netObjects)
            {
                action(netObject.Value);
            }
        }
    }
}
