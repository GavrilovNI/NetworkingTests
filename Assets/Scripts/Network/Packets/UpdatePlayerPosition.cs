using LiteNetLib;
using Network.NetObjects;
using UnityEngine;

namespace Network.Packets
{
    public class UpdatePlayerPosition : NetworkPacket
    {
        public Vector3 Position { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableSequenced;
        public override PacketDirection PacketDirection => PacketDirection.ToServer;

        public UpdatePlayerPosition()
        {

        }
        public UpdatePlayerPosition(Vector3 position)
        {
            Position = position;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            NetworkPlayer player = manager.GetNetworkPlayer(sender);

            if (player.PlayerNetObjectId < 0)
                return;

            UpdateNetObjectPosition packet = new UpdateNetObjectPosition(player.PlayerNetObjectId, Position);

            packet.Apply(manager, sender); // updating player position on server

            manager.SendToAll(packet, sender);
        }
    }
}
