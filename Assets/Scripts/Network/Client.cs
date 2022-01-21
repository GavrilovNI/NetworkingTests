using LiteNetLib;
using LiteNetLib.Utils;
using Network.NetObjects;
using Network.Packets;
using Network.Utils.NetPacketProcessorSystemTypes;
using Network.Utils.NetPacketProcessorUnityTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Network
{
    public class Client : NetworkManager
    {
        [Serializable]
        public class Settings
        {
            public int Port;
            public int UpdateTime;
            public string Password;

            public Settings()
            {
                Password = "";
                UpdateTime = 15;
                Port = 5000;
            }
        }

        [SerializeField] private Settings _settings = new Settings();

        private EventBasedNetListener _listener;
        private NetManager _netClient;

        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

        private NetworkPlayer _localNetworkPlayer;

        [SerializeField] private NetObjectTransformable _playerPrefab;

        public NetObjectTransformable GetLocalPlayerPrefab()
        {

            return _playerPrefab;
        }

        private void Awake()
        {
            _netPacketProcessor.RegisterSystemTypes();
            _netPacketProcessor.RegisterUnityTypes();
            RegisterPackets(_netPacketProcessor);

            NetObjectsContainer = new GameObject("NetObjects").AddComponent<NetObjectsContainer>();
            NetObjectsContainer.transform.SetParent(transform);
            NetObjectsContainer.SetNetManager(this);

            LocalNetObjectsContainer = new GameObject("LocalNetObjects").AddComponent<NetObjectsContainer>();
            LocalNetObjectsContainer.transform.SetParent(transform);
            LocalNetObjectsContainer.SetNetManager(this);

            _listener = new EventBasedNetListener();
            _listener.ConnectionRequestEvent += OnConnectionRequest;
            _listener.NetworkErrorEvent += OnNetworkError;
            _listener.NetworkReceiveEvent += OnNetworkReceive;
            _listener.NetworkReceiveUnconnectedEvent += OnNetworkReceiveUnconnected;
            _listener.PeerConnectedEvent += OnPeerConnected;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;

            _netClient = new NetManager(_listener);
            _netClient.UnconnectedMessagesEnabled = true;
            _netClient.UpdateTime = _settings.UpdateTime;
        }

        private void Start()
        {

            _netClient.Start();
        }

        private void Update()
        {
            _netClient.PollEvents();

            var peer = _netClient.FirstPeer;
            if (peer != null && peer.ConnectionState == ConnectionState.Connected)
            {

            }
            else
            {
                _netClient.SendBroadcast(new byte[] { 1 }, _settings.Port);
            }
        }

        private void OnDestroy()
        {
            if (_netClient != null)
            {
                _netClient.DisconnectAll();
                _netClient.Stop();
            }
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[CLIENT] Connected to " + peer.EndPoint);

            _localNetworkPlayer = new NetworkPlayer(null);
        }

        private void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Debug.Log("[CLIENT] Received error " + socketErrorCode);
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine("[Client] Received data. Processing...");
            _netPacketProcessor.ReadAllPackets(reader, peer);
        }

        private void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.BasicMessage && _netClient.ConnectedPeersCount == 0 && reader.GetInt() == 1)
            {
                //Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
                _netClient.Connect(remoteEndPoint, _settings.Password);
            }
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {

        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[CLIENT] Disconnected. Reason: " + disconnectInfo.Reason);
            NetObjectsContainer.Clear();
            LocalNetObjectsContainer.Clear();
        }

        public override NetworkPlayer GetNetworkPlayer(NetPeer peer)
        {
            return _localNetworkPlayer;
        }

        public override void SendToAll<T>(T networkPacket)
        {
            Send(networkPacket);
        }
        public override void SendToAll<T>(T networkPacket, NetPeer peer)
        {
            if(peer != _netClient.FirstPeer)
                Send(networkPacket);
        }

        public override void Send<T>(NetPeer peer, T networkPacket)
        {
            Send(networkPacket);
        }

        public void Send<T>(T networkPacket) where T : NetworkPacket, new()
        {
            _netPacketProcessor.Send(_netClient.FirstPeer, networkPacket, networkPacket.DeliveryMethod);
        }

        public void SendIfConnected<T>(T networkPacket) where T : NetworkPacket, new()
        {
            if (_netClient.ConnectedPeersCount > 0)
            {
                Send(networkPacket);
            }
        }

        public override void ApplyNetPacket<T>(T packet, NetPeer peer)
        {
            if (packet.PacketDirection == PacketDirection.ToServer)
                return;
            packet.Apply(this, peer);
        }
    }
}