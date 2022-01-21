using LiteNetLib;
using LiteNetLib.Utils;
using Network.NetObjects;
using Network.Packets;
using UnityEngine;

namespace Network
{
    public abstract class NetworkManager : MonoBehaviour
    {
        public NetObjectsContainer NetObjectsContainer { get; protected set; }
        public NetObjectsContainer LocalNetObjectsContainer { get; protected set; }


        public abstract NetworkPlayer GetNetworkPlayer(NetPeer peer);

        public abstract void SendToAll<T>(T networkPacket) where T : NetworkPacket, new();
        public abstract void SendToAll<T>(T networkPacket, NetPeer except) where T : NetworkPacket, new();
        public abstract void Send<T>(NetPeer peer, T networkPacket) where T : NetworkPacket, new();

        public abstract void ApplyNetPacket<T>(T packet, NetPeer peer) where T: NetworkPacket, new();

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
