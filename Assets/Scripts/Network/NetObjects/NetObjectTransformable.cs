using Network.Test;
using System;
using UnityEngine;

namespace Network.NetObjects
{
    public class NetObjectTransformable : NetObject
    {
        public InterplotatingChain<Vector3> PositionChain { get; private set; }

        public float CreationTime { get; private set; }
        public float LifeTime => Time.realtimeSinceStartup - CreationTime;

        private void Awake()
        {
            PositionChain = new InterplotatingChain<Vector3>(Vector3.Lerp);

            CreationTime = Time.realtimeSinceStartup;
        }

        private void Start()
        {

            PositionChain.Value = transform.position;
            PositionChain.ValueChanged += pos => transform.position = pos;

            Transform model = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            model.SetParent(this.transform);
            model.localPosition = Vector3.zero;
        }

        private void FixedUpdate()
        {
            PositionChain.Move(Time.fixedDeltaTime);
        }
    }
}
