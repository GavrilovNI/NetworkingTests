using LiteNetLib;
using UnityEngine;
using Network.NetObjects;
using System;

namespace Network.Packets
{
    public class UpdateNetObjectPosition : NetworkPacket
    {
        public int NetObjectId { get; set; }
        public Vector3 NewPosition { get; set; }
        public float DeltaTime { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.Sequenced;
        public override PacketDirection PacketDirection => PacketDirection.ToClient;

        public UpdateNetObjectPosition()
        {
            
        }
        public UpdateNetObjectPosition(int netObjectId, Vector3 newPosition, float deltaTime)
        {
            NetObjectId = netObjectId;
            NewPosition = newPosition;
            DeltaTime = deltaTime;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            NetObjectsContainer netObjectsContainer = manager.NetObjectsContainer;

            if (netObjectsContainer.HasNetObject(NetObjectId) == false)
                return;

            NetObjectTransformable netObject = netObjectsContainer.GetNetObject(NetObjectId) as NetObjectTransformable;

            if (netObject != null)
            {
                netObject.PositionChain.AddToChain(new InterpolatingNode<Vector3>(NewPosition, DeltaTime));
            }
        }
    }
}
