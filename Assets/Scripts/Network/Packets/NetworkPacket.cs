using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    public enum PacketDirection
    {
        ToServer,
        ToClient,
        Any
    }

    public abstract class NetworkPacket
    {
        public virtual DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableSequenced;
        public virtual PacketDirection PacketDirection => PacketDirection.Any;


        public abstract void Apply(NetworkManager manager, NetPeer sender);
    }
}
