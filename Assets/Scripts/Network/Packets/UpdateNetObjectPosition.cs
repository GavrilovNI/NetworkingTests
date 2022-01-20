using LiteNetLib;
using UnityEngine;
using Network.NetObjects;

namespace Network.Packets
{
    public class UpdateNetObjectPosition : NetworkPacket
    {
        public int NetObjectId { get; set; }
        public Vector3 NewPosition { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableSequenced;
        public override PacketDirection PacketDirection => PacketDirection.ToClient;

        public UpdateNetObjectPosition()
        {
            
        }
        public UpdateNetObjectPosition(int netObjectId, Vector3 newPosition)
        {
            NetObjectId = netObjectId;
            NewPosition = newPosition;
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
                netObject.Position = NewPosition;
        }
    }
}
