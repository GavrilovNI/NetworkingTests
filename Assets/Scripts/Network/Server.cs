using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using System;
using Network.Packets;
using Network.NetObjects;
using Network.Utils.NetPacketProcessorUnityTypes;
using Network.Utils.NetPacketProcessorSystemTypes;
using System.Linq;

namespace Network
{
    [RequireComponent(typeof(NetObjectsContainer))]
    public class Server : NetworkManager
    {
        [Serializable]
        public class Settings
        {
            public int Port;
            public int UpdateTime;
            public int MaxConnections;
            public string Password;

            public Settings()
            {
                Password = "";
                UpdateTime = 15;
                MaxConnections = 20;
                Port = 5000;
            }
        }

        [SerializeField] private Settings _Settings = new Settings();

        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
        private EventBasedNetListener _listener;
        private NetManager _netServer;
        private NetDataWriter _dataWriter;

        private Dictionary<NetPeer, NetworkPlayer> _players = new Dictionary<NetPeer, NetworkPlayer>();



        private void Awake()
        {
            _netPacketProcessor.RegisterSystemTypes();
            _netPacketProcessor.RegisterUnityTypes();
            RegisterPackets(_netPacketProcessor);

            NetObjectsContainer = GetComponent<NetObjectsContainer>();
            NetObjectsContainer.SetNetManager(this);

            _listener = new EventBasedNetListener();
            _listener.ConnectionRequestEvent += OnConnectionRequest;
            _listener.NetworkErrorEvent += OnNetworkError;
            _listener.NetworkReceiveEvent += OnNetworkReceive;
            _listener.NetworkReceiveUnconnectedEvent += OnNetworkReceiveUnconnected;
            _listener.PeerConnectedEvent += OnPeerConnected;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;

            _dataWriter = new NetDataWriter();
            _netServer = new NetManager(_listener);
            _netServer.BroadcastReceiveEnabled = true;
            _netServer.UpdateTime = _Settings.UpdateTime;
        }

        private void Start()
        {
            _netServer.Start(_Settings.Port);
        }

        private void Update()
        {
            _netServer.PollEvents();
        }

        private void OnDestroy()
        {
            if (_netServer != null)
            {
                _netServer.DisconnectAll();
                _netServer.Stop();
            }
        }

        //can be called after few players connected and putted to _netServer.ConnectedPeerList 
        private void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[SERVER] New peer connected " + peer.EndPoint);

            NetworkPlayer player = new NetworkPlayer(peer);
            _players.Add(peer, player);

            int x = -1;

            NetObjectsContainer.Foreach(netObject => Send(peer, new CreateNetObject(netObject)));

            NetObjectTransformable playerObject = NetObjectsContainer.CreateNetObject<NetObjectTransformable>();
            player.PlayerNetObjectId = playerObject.Id;
            x = playerObject.Id;

            SendToAll(new CreateNetObject(playerObject), peer);
            Send(peer, new SpawnLocalPlayer(playerObject.Id));
        }

        private void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Debug.Log("[SERVER] Received error " + socketErrorCode);
        }

        private void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.Broadcast)
            {
                Debug.Log("[SERVER] Received discovery request. Send discovery response");
                NetDataWriter resp = new NetDataWriter();
                resp.Put(1);
                _netServer.SendUnconnectedMessage(resp, remoteEndPoint);
            }
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            Debug.Log("[SERVER] Peer trying to connect: " + request.RemoteEndPoint);
            if (_netServer.ConnectedPeersCount < _Settings.MaxConnections)
            {
                if (_Settings.Password == String.Empty)
                {
                    Debug.Log("[SERVER] Connection request accepted.");
                    request.Accept();
                }
                else
                {
                    Debug.Log("[SERVER] Connection request rejected: Wrong password.");
                    request.AcceptIfKey(_Settings.Password);
                }
            }
            else
            {
                Debug.Log("[SERVER] Connection request rejected: Connections limit.");
                request.Reject();
            }
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[SERVER] Peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);

            int playerNetObjectId = _players[peer].PlayerNetObjectId;
            SendToAll(new DestroyNetObject(playerNetObjectId));
            NetObjectsContainer.DestroyNetObject(playerNetObjectId);

            _players.Remove(peer);
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine("[Server] Received data. Processing...");
            _netPacketProcessor.ReadAllPackets(reader, peer);
        }

        public override void SendToAll<T>(T networkPacket, NetPeer except)
        {
            //OnPeerConnected can be called after few players connected and already putted to _netServer.ConnectedPeerList 
            //so, using _players.Keys instead of _netServer.ConnectedPeerList
            //because players beeing putted to _players.Keys after every call of OnPeerConnected
            //so only one player can be added to _players between 2 calls of OnPeerConnected
            foreach (NetPeer peer in _players.Keys.Except(new NetPeer[] { except }))
            {
                Send(peer, networkPacket);
            }
        }
        public override void SendToAll<T>(T networkPacket)
        {
            //OnPeerConnected can be called after few players connected and already putted to _netServer.ConnectedPeerList 
            //so, using _players.Keys instead of _netServer.ConnectedPeerList
            //because players beeing putted to _players.Keys after every call of OnPeerConnected
            //so only one player can be added to _players between 2 calls of OnPeerConnected
            foreach (NetPeer peer in _players.Keys)
            {
                Send(peer, networkPacket);
            }
        }

        public override void Send<T>(NetPeer peer, T networkPacket)
        {
            _netPacketProcessor.Send<T>(peer, networkPacket, networkPacket.DeliveryMethod);
        }

        public override NetworkPlayer GetNetworkPlayer(NetPeer peer)
        {
            return _players[peer];
        }

        public override void ApplyNetPacket<T>(T packet, NetPeer peer)
        {
            if (packet.PacketDirection == PacketDirection.ToClient)
                return;
            packet.Apply(this, peer);
        }
    }
}