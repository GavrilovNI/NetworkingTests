using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Network.NetObjects
{
    public class NetObjectsContainer : MonoBehaviour
    {
        private int _nextId = 0;
        private Dictionary<int, NetObject> _netObjects = new Dictionary<int, NetObject>();

        private NetworkManager _networkManager;

        public void SetNetManager(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        private void InitializeNetObject(NetObject netObject)
        {
            InitializeNetObject(netObject, _nextId++);
        }
        private void InitializeNetObject(NetObject netObject, int id)
        {
            netObject.Initialize(id, _networkManager);
            _netObjects.Add(netObject.Id, netObject);

            netObject.transform.SetParent(transform);
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

        public T CreateNetObject<T>(T prefab, int id) where T : NetObject
        {
            T netObject = MonoBehaviour.Instantiate(prefab);
            InitializeNetObject(netObject, id);
            netObject.name = prefab.name + " " + netObject.name;
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

        private void DestroyNetObjectButKeepInDictionary(int id)
        {
            GameObject.Destroy(_netObjects[id].gameObject);
        }

        public void DestroyNetObject(int id)
        {
            DestroyNetObjectButKeepInDictionary(id);
            _netObjects.Remove(id);
        }

        public void Foreach(Action<NetObject> action)
        {
            foreach(var netObject in _netObjects)
            {
                action(netObject.Value);
            }
        }

        // In SafeForeach you can destroy NetObjects
        public void SafeForeach(Action<NetObject> action)
        {
            _netObjects.ToList().ForEach(p => action(p.Value));
        }

        public void Clear()
        {
            SafeForeach(netObject => DestroyNetObjectButKeepInDictionary(netObject.Id));
            _netObjects.Clear();

            _nextId = 0;
        }
    }
}
