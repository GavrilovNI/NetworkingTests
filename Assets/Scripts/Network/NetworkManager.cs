using LiteNetLib;
using LiteNetLib.Utils;
using Network.NetObjects;
using Network.Packets;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public abstract class NetworkManager : MonoBehaviour
    {

        public NetObjectsContainer NetObjectsContainer { get; protected set; }
        public NetObjectsContainer LocalNetObjectsContainer { get; protected set; }


        public abstract NetworkPlayer GetNetworkPlayer(NetPeer peer);


        
        public abstract void Send<T>(NetPeer peer, T networkPacket) where T : NetworkPacket, new();

        public virtual void Send<T>(IEnumerable<NetPeer> peers, T networkPacket) where T : NetworkPacket, new()
        {
            foreach (NetPeer peer in peers)
            {
                Send(peer, networkPacket);
            }
        }
        public abstract void SendToAll<T>(T networkPacket) where T : NetworkPacket, new();
        public abstract void SendToAll<T>(T networkPacket, NetPeer except) where T : NetworkPacket, new();

        public abstract void ApplyNetPacket<T>(T packet, NetPeer sender) where T: NetworkPacket, new();

        protected void InitializeNetObjectContainers()
        {
            NetObjectsContainer = new GameObject("NetObjects").AddComponent<NetObjectsContainer>();
            NetObjectsContainer.transform.SetParent(transform);
            NetObjectsContainer.SetNetManager(this);

            LocalNetObjectsContainer = new GameObject("LocalNetObjects").AddComponent<NetObjectsContainer>();
            LocalNetObjectsContainer.transform.SetParent(transform);
            LocalNetObjectsContainer.SetNetManager(this);
        }

        public void RegisterPackets(NetPacketProcessor netPacketProcessor)
        {
            netPacketProcessor.SubscribeReusable<UpdateNetObjectPosition, NetPeer>(ApplyNetPacket);
            netPacketProcessor.SubscribeReusable<UpdatePlayerPosition, NetPeer>(ApplyNetPacket);
            netPacketProcessor.SubscribeReusable<CreateNetObject, NetPeer>(ApplyNetPacket);
            netPacketProcessor.SubscribeReusable<DestroyNetObject, NetPeer>(ApplyNetPacket);
            netPacketProcessor.SubscribeReusable<SpawnLocalPlayer, NetPeer>(ApplyNetPacket);
            netPacketProcessor.SubscribeReusable<CreateNetObjectRealPositionShower, NetPeer>(ApplyNetPacket);
        }
    }

    
}
