using Network.Packets;
using UnityEngine;

namespace Network.Test
{
    [RequireComponent(typeof(Client))]
    public class PlayerMovementTest : MonoBehaviour
    {
        private Client _client;
        private float _position = 0;

        [SerializeField] private float _speed = 3f;

        private void Awake()
        {
            _client = GetComponent<Client>();
        }

        private void FixedUpdate()
        {
            float directionMultiplier = 0;
            if (Input.GetKey(KeyCode.A))
            {
                directionMultiplier -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                directionMultiplier += 1;
            }
            Debug.Log(directionMultiplier);
            if(directionMultiplier != 0)
            {
                _position += directionMultiplier * Time.fixedDeltaTime * _speed;
                _position = Mathf.Clamp(_position, 0f, 10f);
                _client.Send(new UpdatePlayerPosition(_position));
            }
        }
    }
}
