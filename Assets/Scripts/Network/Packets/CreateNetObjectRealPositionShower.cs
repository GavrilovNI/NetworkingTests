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
    public class CreateNetObjectRealPositionShower : NetworkPacket
    {
        public int NetObjectId { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableOrdered;
        public override PacketDirection PacketDirection => PacketDirection.ToClient;

        public CreateNetObjectRealPositionShower()
        {

        }
        public CreateNetObjectRealPositionShower(int netObjectId)
        {
            NetObjectId = netObjectId;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            var target = manager.NetObjectsContainer.GetNetObject(NetObjectId) as NetObjectTransformable;
            var netObject = manager.LocalNetObjectsContainer.CreateNetObject<NetObjectRealPositionShower>();
            netObject.Target = target;
        }
    }
}
