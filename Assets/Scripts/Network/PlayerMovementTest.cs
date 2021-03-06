using Network.NetObjects;
using Network.Packets;
using Network.Utils.VectorMath;
using UnityEngine;

namespace Network.Test
{
    [RequireComponent(typeof(NetObjectTransformable))]
    public class PlayerMovementTest : MonoBehaviour
    {
        private NetObjectTransformable _netObject;

        private float _lastTimePositionSent;

        [SerializeField] private float _speed = 3f;

        private void Awake()
        {
            _netObject = GetComponent<NetObjectTransformable>();
        }

        private void FixedUpdate()
        {
            Vector3 directionMultiplier = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
            {
                directionMultiplier += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                directionMultiplier += Vector3.right;
            }
            if (Input.GetKey(KeyCode.W))
            {
                directionMultiplier += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                directionMultiplier += Vector3.back;
            }


            bool isMoving = directionMultiplier != Vector3.zero;
            if (isMoving)
            {
                Vector3 newPosition = transform.position + directionMultiplier.normalized * Time.fixedDeltaTime * _speed;

                newPosition = newPosition.Clamp(Vector3.zero, Vector3.one * 10);

                transform.position = newPosition;
                Client client = _netObject.NetworkManager as Client;
                client?.Send(new UpdatePlayerPosition(transform.position));

                _lastTimePositionSent = Time.realtimeSinceStartup;
            }
        }
    }
}
