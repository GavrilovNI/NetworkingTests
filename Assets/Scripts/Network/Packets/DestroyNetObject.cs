using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    public class DestroyNetObject : NetworkPacket
    {
        public int NetObjectId { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableOrdered;
        public override PacketDirection PacketDirection => PacketDirection.ToClient;

        public DestroyNetObject()
        {

        }
        public DestroyNetObject(int netObjectId)
        {
            NetObjectId = netObjectId;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            manager.NetObjectsContainer.DestroyNetObject(NetObjectId);
        }
    }
}
