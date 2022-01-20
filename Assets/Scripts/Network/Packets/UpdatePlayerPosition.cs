using LiteNetLib;
using Network.NetObjects;
using UnityEngine;

namespace Network.Packets
{
    public class UpdatePlayerPosition : NetworkPacket
    {
        public float Position { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableSequenced;
        public override PacketDirection PacketDirection => PacketDirection.ToServer;

        public UpdatePlayerPosition()
        {

        }
        public UpdatePlayerPosition(float position)
        {
            Position = position;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            NetworkPlayer player = manager.GetNetworkPlayer(sender);

            if (player.PlayerNetObjectId < 0)
                return;

            Vector3 newPosition = new Vector3(Mathf.Clamp(Position, 0, 10), 0, 0);

            UpdateNetObjectPosition packet = new UpdateNetObjectPosition(player.PlayerNetObjectId, newPosition);

            packet.Apply(manager, sender); // updating player position on server

            manager.SendToAll(packet);
        }
    }
}
