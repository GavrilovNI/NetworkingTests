using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class NetworkPlayer
    {
        public readonly NetPeer Peer;

        public int PlayerNetObjectId = -1;

        public NetworkPlayer(NetPeer netPeer)
        {
            Peer = netPeer;
        }

    }
}
