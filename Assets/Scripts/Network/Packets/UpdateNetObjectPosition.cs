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
        public float TimeSinceObjectCreated { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableSequenced;
        public override PacketDirection PacketDirection => PacketDirection.ToClient;

        public UpdateNetObjectPosition()
        {
            
        }
        public UpdateNetObjectPosition(int netObjectId, Vector3 newPosition, float timeSinceObjectCreated)
        {
            NetObjectId = netObjectId;
            NewPosition = newPosition;
            TimeSinceObjectCreated = timeSinceObjectCreated;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            NetObjectsContainer netObjectsContainer = manager.NetObjectsContainer;

            if (netObjectsContainer.HasNetObject(NetObjectId) == false)
                return;

#nullable enable
            NetObjectTransformable? netObject = netObjectsContainer.GetNetObject(NetObjectId) as NetObjectTransformable;
#nullable disable

            if (netObject != null)
            {
                Vector3 lastPos = netObject.PositionChain.GetValue(netObject.PositionChain.Length - 1).Value;
                float dist = Vector3.Distance(lastPos, NewPosition);
                float minSpeed = 1f;
                float maxDeltaTime = dist / minSpeed;

                float deltaTime = TimeSinceObjectCreated - netObject.LastTimePositionChanged;
                if(deltaTime > maxDeltaTime)
                    deltaTime = maxDeltaTime;
                if (deltaTime < 0)
                    deltaTime = 0;

                netObject.PositionChain.AddToChain(new InterpolatingNode<Vector3>(NewPosition, deltaTime));
                //netObject.PositionChain.Normalize();
                netObject.LastTimePositionChanged = TimeSinceObjectCreated;
            }
        }
    }
}
