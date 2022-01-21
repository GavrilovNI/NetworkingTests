using LiteNetLib;
using Network.NetObjects;
using UnityEngine;

namespace Network.Packets
{
    public class UpdatePlayerPosition : NetworkPacket
    {
        public Vector3 Position { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.Sequenced;
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

            const float playerMovementDeltaTime = 0.02f;
            UpdateNetObjectPosition packet = new UpdateNetObjectPosition(player.PlayerNetObjectId, Position, playerMovementDeltaTime);

            //uncomment to lost packets
            /*var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = Position;
            MonoBehaviour.Destroy(go, 1);
            */

            packet.Apply(manager, sender); // updating player position on server

            manager.SendToAll(packet, sender);
        }
    }
}
