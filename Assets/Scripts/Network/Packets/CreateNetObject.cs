using LiteNetLib;
using Network.NetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Packets
{
    public class CreateNetObject : NetworkPacket
    {
        public Type Type { get; set; }
        public int NetObjectId { get; set; }

        public Vector3 Position { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableOrdered;
        public override PacketDirection PacketDirection => PacketDirection.ToClient;

        public CreateNetObject()
        {

        }
        public CreateNetObject(NetObject netObject)
        {
            Type = netObject.GetType();
            NetObjectId = netObject.Id;
            Position = netObject.transform.position;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            var netObject = manager.NetObjectsContainer.CreateNetObject(Type, NetObjectId);
            netObject.transform.position = Position;
        }
    }
}
