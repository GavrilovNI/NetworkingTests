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
        private NetManager _netManager;

        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

        private NetworkPlayer _localNetworkPlayer;

        [SerializeField] private NetObjectTransformable _playerPrefab;

        public NetObjectTransformable GetLocalPlayerPrefab()
        {

            return _playerPrefab;
        }

        private void IntializeListener()
        {
            _listener = new EventBasedNetListener();
            _listener.ConnectionRequestEvent += OnConnectionRequest;
            _listener.NetworkErrorEvent += OnNetworkError;
            _listener.NetworkReceiveEvent += OnNetworkReceive;
            _listener.NetworkReceiveUnconnectedEvent += OnNetworkReceiveUnconnected;
            _listener.PeerConnectedEvent += OnPeerConnected;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;
        }


        private void Awake()
        {
            _netPacketProcessor.RegisterSystemTypes();
            _netPacketProcessor.RegisterUnityTypes();
            RegisterPackets(_netPacketProcessor);

            InitializeNetObjectContainers();
            IntializeListener();

            _netManager = new NetManager(_listener);
            _netManager.UnconnectedMessagesEnabled = true;
            _netManager.UpdateTime = _settings.UpdateTime;
        }

        private void Start()
        {

            _netManager.Start();
        }

        private void Update()
        {
            _netManager.PollEvents();

            var peer = _netManager.FirstPeer;
            if (peer != null && peer.ConnectionState == ConnectionState.Connected)
            {

            }
            else
            {
                _netManager.SendBroadcast(new byte[] { 1 }, _settings.Port);
            }
        }

        private void OnDestroy()
        {
            if (_netManager != null)
            {
                _netManager.DisconnectAll();
                _netManager.Stop();
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
            if (messageType == UnconnectedMessageType.BasicMessage && _netManager.ConnectedPeersCount == 0 && reader.GetInt() == 1)
            {
                //Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
                _netManager.Connect(remoteEndPoint, _settings.Password);
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
            if(peer != _netManager.FirstPeer)
                Send(networkPacket);
        }

        public override void Send<T>(NetPeer peer, T networkPacket)
        {
            Send(networkPacket);
        }

        public void Send<T>(T networkPacket) where T : NetworkPacket, new()
        {
            _netPacketProcessor.Send(_netManager.FirstPeer, networkPacket, networkPacket.DeliveryMethod);
        }

        public void SendIfConnected<T>(T networkPacket) where T : NetworkPacket, new()
        {
            if (_netManager.ConnectedPeersCount > 0)
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