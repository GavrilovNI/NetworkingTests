using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class InterpolatingNode<T>
    {
        private float _time;

        public T Value { get; set; }
        public float Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value < 0)
                    throw new Exception("Time can't be less than 0.");
                _time = value;
            }
        }

        public InterpolatingNode()
        {
            Value = default;
            Time = 0;
        }
        public InterpolatingNode(T value, float time)
        {
            Value = value;
            Time = time;
        }
    }

    public class InterplotatingChain<T>
    {
        public event Action<T> ValueChanged;
        public event Action<InterpolatingNode<T>> ChainGrowed;

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                _nodes.Clear();
                _nodes.Add(new InterpolatingNode<T>(_value, 0f));

                ValueChanged?.Invoke(_value);
            }
        }

        private float _time = 0;

        private List<InterpolatingNode<T>> _nodes = new List<InterpolatingNode<T>>();

        private Func<T, T, float, T> _interpolator;
        //private Func<T, T, float> _sqrDistance;
        public int Length => _nodes.Count;

        public InterplotatingChain(Func<T, T, float, T> interpolator/*, Func<T, T, float> sqrDistance*/)
        {
            _interpolator = interpolator;
            //_sqrDistance = sqrDistance;
            Value = default(T);
        }

        public InterpolatingNode<T> GetValue(int index)
        {
            return _nodes[index];
        }

        public void AddToChain(InterpolatingNode<T> node)
        {
            _nodes.Add(node);
            ChainGrowed?.Invoke(node);
        }

        public void Move(float deltaTime)
        {
            if(deltaTime < 0)
                throw new ArgumentOutOfRangeException(nameof(deltaTime));

            if (Length < 2)
                return;

            _time += deltaTime;
            InterpolateValue();
        }

        private void InterpolateValue()
        {
            if (Length < 2)
            {
                _time = 0;
                return;
            }

            if(_time >= _nodes[1].Time)
            {
                _time -= _nodes[1].Time;
                _value = _nodes[1].Value;
                _nodes.RemoveAt(0);

                ValueChanged?.Invoke(_value);
                InterpolateValue();

                return;
            }

            if(_time > 0)
            {
                _value = _interpolator(_nodes[0].Value, _nodes[1].Value, _time / _nodes[1].Time);
                ValueChanged?.Invoke(_value);
            }
        }

        /*public void Normalize()
        {
            if (Length < 2)
                return;

            float time = _times.Sum();
            T prev = _values[0];
            float sqrDistanceTotal = _values.Sum(value =>
            {
                float sqrDistance = _sqrDistance(value, prev);
                prev = value;
                return sqrDistance;
            });

            if(sqrDistanceTotal == 0)
            {
                return;
            }

            for (int i = 0; i < _times.Count; i++)
            {
                _times[i] = time * _sqrDistance(_values[i], _values[i + 1]) / sqrDistanceTotal;
            }
        }*/
    }
}
