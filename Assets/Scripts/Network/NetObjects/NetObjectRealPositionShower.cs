using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.NetObjects
{
    public class NetObjectRealPositionShower : NetObject
    {
        private GameObject _model;
#nullable enable
        private NetObjectTransformable? _target;

        public NetObjectTransformable? Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (_target != null)
                {
                    _target.PositionChain.ChainGrowed -= OnPositionChainGrowed;
                }

                _target = value;

                if(_target == null)
                {
                    _model.SetActive(false);
                }
                else
                {
                    _model.SetActive(true);
                    _target.PositionChain.ChainGrowed += OnPositionChainGrowed;

                    InterpolatingNode<Vector3> lastNode = _target.PositionChain.GetValue(_target.PositionChain.Length - 1);
                    OnPositionChainGrowed(lastNode);
                }
            }
        }
#nullable disable

        private void Awake()
        {
            _model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _model.transform.SetParent(this.transform);
            _model.transform.localPosition = Vector3.zero;
        }

        private void OnPositionChainGrowed(InterpolatingNode<Vector3> node)
        {
            transform.position = node.Value;
        }
    }
}
