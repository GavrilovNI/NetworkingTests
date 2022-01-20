using Network.Test;
using System;
using UnityEngine;

namespace Network.NetObjects
{
    public class NetObjectTransformable : NetObject
    {
        public InterplotatingChain<Vector3> PositionChain { get; private set; }
        public float LastTimePositionChanged;

        public float CreationTime { get; private set; }
        public float LifeTime => Time.realtimeSinceStartup - CreationTime;

        public Transform _realPosition;

        private void Awake()
        {
            PositionChain = new InterplotatingChain<Vector3>(Vector3.Lerp, (a, b) => (a - b).sqrMagnitude);

            CreationTime = Time.realtimeSinceStartup;
        }

        private void Start()
        {

            PositionChain.Value = transform.position;
            PositionChain.ValueChanged += pos => transform.position = pos;
            LastTimePositionChanged = Time.realtimeSinceStartup;

            Transform model = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            model.SetParent(this.transform);
            model.localPosition = Vector3.zero;

            if (GetComponent<PlayerMovementTest>())
            {
                _realPosition = new GameObject().transform;
            }
            else
            {
                _realPosition = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            }
            _realPosition.name = "realpos" + transform.name;
            _realPosition.position = model.position;
        }

        private void FixedUpdate()
        {
            PositionChain.Update(Time.fixedDeltaTime);
        }

        private void OnDestroy()
        {
            GameObject.Destroy(_realPosition.gameObject);
        }
    }
}
