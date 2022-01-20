using LiteNetLib;
using Network.NetObjects;
using Network.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Packets
{
    public class SpawnLocalPlayer : NetworkPacket
    {
        public int PlayerNetObjectId { get; set; }

        public override DeliveryMethod DeliveryMethod => DeliveryMethod.ReliableOrdered;
        public override PacketDirection PacketDirection => PacketDirection.ToClient;

        public SpawnLocalPlayer()
        {

        }
        public SpawnLocalPlayer(int playerNetObjectId)
        {
            PlayerNetObjectId = playerNetObjectId;
        }

        public override void Apply(NetworkManager manager, NetPeer sender)
        {
            Client client = manager as Client;

            NetObjectTransformable playerPrefab = client.GetLocalPlayerPrefab();

            NetObjectTransformable player = manager.NetObjectsContainer.CreateNetObject(playerPrefab, PlayerNetObjectId);
        }
    }
}
