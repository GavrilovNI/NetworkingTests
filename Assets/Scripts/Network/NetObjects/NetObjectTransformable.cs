using System;
using UnityEngine;

namespace Network.NetObjects
{
    public class NetObjectTransformable : NetObject
    {
        public event Action<Vector3, Vector3> PositionChanged;

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                Vector3 oldPosition = Position;
                transform.position = value;
                PositionChanged?.Invoke(oldPosition, value);
            }
        }

        private void Start()
        {
            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.SetParent(this.transform);
        }
    }
}
