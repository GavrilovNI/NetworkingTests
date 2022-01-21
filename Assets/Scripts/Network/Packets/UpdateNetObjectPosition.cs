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

#nullable enable
            NetObjectTransformable? netObject = netObjectsContainer.GetNetObject(NetObjectId) as NetObjectTransformable;
#nullable disable

            if (netObject != null)
            {
                /*Vector3 lastPos = netObject.PositionChain.GetValue(netObject.PositionChain.Length - 1).Value;
                float dist = Vector3.Distance(lastPos, NewPosition);
                float minSpeed = 1f;
                float maxDeltaTime = dist / minSpeed;

                float updateTime = Time.realtimeSinceStartup - sender.Ping / 1000f;
                float deltaTime = updateTime - netObject.LastTimePositionChanged;
                //if(deltaTime > maxDeltaTime)
                //    deltaTime = maxDeltaTime;
                if (deltaTime < 0)
                    deltaTime = 0;
                Debug.Log("Delta " + deltaTime);
                Debug.Log("Distance " + dist);
                Debug.Log("Speed " + dist/deltaTime);
                if(dist > 0.5f)
                    Debug.Log("-----------------------------------------------");*/

                netObject.PositionChain.AddToChain(new InterpolatingNode<Vector3>(NewPosition, DeltaTime));
                //netObject.PositionChain.Normalize();
                //netObject.LastTimePositionChanged = updateTime;
            }
        }
    }
}
